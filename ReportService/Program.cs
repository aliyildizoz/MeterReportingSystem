using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReportService.Data;
using Share;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

                       });
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

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportContext>();
    db.Database.Migrate();
}
app.Run();
