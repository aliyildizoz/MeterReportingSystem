using MeterService;
using MeterService.Data;
using MeterService.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Grpc.AspNetCore.Web;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithOrigins("http://localhost:5002", "http://localhost:4200");
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
}); 
builder.Services.AddAutoMapper(typeof(Program).Assembly);



builder.Services.AddDbContext<MeterContext>(
                       options =>
                       {
                           var connectionString = builder.Configuration.GetConnectionString("SqlServer");
                           if (!builder.Environment.IsDevelopment())
                           {
                               var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
                               connectionString = string.Format(connectionString, password);
                           }
                           options.UseSqlServer(connectionString);

                       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();


app.UseGrpcWeb();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();
app.MapGrpcService<MeterService.gRPC.Services.MeterGrpcService>().EnableGrpcWeb().RequireCors("AllowAll");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MeterContext>();
    db.Database.Migrate();
}

app.Run();
