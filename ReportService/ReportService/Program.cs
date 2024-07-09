using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.gRPC;
using ReportService.Hubs;
using ReportService.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.WithOrigins("http://localhost:4200")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
}));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddMeterGrpcService(builder.Configuration);

builder.Services.AddDbContext<ReportContext>(
                       options =>
                       {
                           var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
                           if (!builder.Environment.IsDevelopment())
                           {
                               var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
                               var db = Environment.GetEnvironmentVariable("POSTGRES_DB");
                               connectionString = string.Format(connectionString, password,db);
                           }
                           options.UseNpgsql(connectionString);

                       }, ServiceLifetime.Transient);

builder.Services.AddRabbitMQServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles();
app.MapHub<ReportNotificationHub>("/report-notification-hub");

app.MapControllers();


Thread.Sleep(30000);
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportContext>();
    db.Database.Migrate();
}
app.Run();
