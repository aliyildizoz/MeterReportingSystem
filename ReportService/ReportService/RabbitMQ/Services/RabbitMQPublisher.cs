using RabbitMQ.Client;
using ReportService.Models;
using System.Text;
using System.Text.Json;

namespace ReportService.RabbitMQ.Services
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(RabbitMQReportRequestModel requestModel)
        {
            var channel = _rabbitMQClientService.Connect();

            var bodyString = JsonSerializer.Serialize(requestModel);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingMeterReport, basicProperties: properties, body: bodyByte);
        }
    }
}