using ReportService.Models;

namespace ReportService.RabbitMQ.Services
{
    public interface IRabbitMQPublisher
    {
        void Publish(RabbitMQReportRequestModel requestModel);
    }
}
