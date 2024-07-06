using MeterService;

namespace ReportService.gRPC.Extensions
{
    public static class RabbitMQServiceCollectionExtension
    {
        public static void AddMeterGrpcService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddGrpcClient<MeterGrpcService.MeterGrpcServiceClient>(c =>
            {
                c.Address = new Uri(configuration.GetConnectionString("MeterService"));
            });

            services.AddSingleton<ReportService.gRPC.Services.IMeterGrpcService, ReportService.gRPC.Services.MeterGrpcService>();
        }
    }
}
