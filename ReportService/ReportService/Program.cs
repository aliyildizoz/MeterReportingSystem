using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using MeterService;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.BackroundServices;
using ReportService.Data;
using Share;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddGrpcClient<MetergRPCService.MetergRPCServiceClient>(c =>
{
    c.Address = new Uri(builder.Configuration.GetConnectionString("MeterService"));
});


builder.Services.AddHostedService<RabbitMQConsumer>();

builder.Services.AddDbContext<ReportContext>(
                       options =>
                       {
                           var connectionString = builder.Configuration.GetConnectionString("SqlServer");
                           if (!builder.Environment.IsDevelopment())
                           {
                               var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
                               connectionString = string.Format(connectionString, password);
                           }
                           options.UseSqlServer(connectionString);

                       }, ServiceLifetime.Singleton);
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportContext>();
    db.Database.Migrate();
}
app.Run();
