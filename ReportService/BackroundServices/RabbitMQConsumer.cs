using RabbitMQ.Client.Events;
using Share;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Models;

namespace ReportService.BackroundServices
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ReportContext _reportContext;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, RabbitMQClientService rabbitMQClientService, ReportContext reportContext)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _reportContext = reportContext;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Thread.Sleep(30000);
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);
            consumer.Received += Consumer_Received;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var requestModel = JsonSerializer.Deserialize<RabbitMQReportRequestModel>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            //todo:get data from meter service
            //todo:create excel

            _channel.BasicAck(@event.DeliveryTag, false);
        }


    }
}
