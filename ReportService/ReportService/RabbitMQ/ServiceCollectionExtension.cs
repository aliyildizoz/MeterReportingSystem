using RabbitMQ.Client;
using ReportService.RabbitMQ.Services;

namespace ReportService.RabbitMQ
{
    public static class ServiceCollectionExtension
    {
        public static void AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
            services.AddHostedService<RabbitMQConsumer>();
            services.AddSingleton<RabbitMQClientService>();
            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
        }
    }
}
