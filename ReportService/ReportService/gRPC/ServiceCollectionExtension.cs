using MeterService;

namespace ReportService.gRPC
{
    public static class ServiceCollectionExtension
    {
        public static void AddMeterGrpcService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<MeterGrpcService.MeterGrpcServiceClient>(c =>
            {
                c.Address = new Uri(configuration.GetConnectionString("MeterService"));
            });

            services.AddSingleton<Services.IMeterGrpcService, Services.MeterGrpcService>();
        }
    }
}
