using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.gRPC;
using ReportService.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithOrigins("http://localhost:4200");
}));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMeterGrpcService(builder.Configuration);

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

                       }, ServiceLifetime.Transient);

builder.Services.AddRabbitMQServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportContext>();
    db.Database.Migrate();
}
app.Run();
