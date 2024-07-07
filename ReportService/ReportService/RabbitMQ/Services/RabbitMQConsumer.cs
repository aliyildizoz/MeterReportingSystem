using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using ReportService.Data;
using ReportService.Models;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using MeterService;
using Google.Protobuf.WellKnownTypes;
using ReportService.gRPC.Services;
using ReportService.Helpers;

namespace ReportService.RabbitMQ.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMeterGrpcService _metergGrpcService;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider, IMeterGrpcService metergGrpcService)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
            _metergGrpcService = metergGrpcService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Thread.Sleep(3000);
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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var requestModel = JsonSerializer.Deserialize<RabbitMQReportRequestModel>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                    var response = await _metergGrpcService.GetMetersBySerialNumberAsync(new MeterReadingRequest() { SerialNumber = requestModel.SerialNumber });

                    var filePath = ExcelHelper.CreateExcel(response.MeterReadingDtos.ToList(), requestModel.SerialNumber);
                    var reportContext = scope.ServiceProvider.GetService<ReportContext>(); 
                    var report = await reportContext.ReportRequests.FirstOrDefaultAsync(x => x.Id == requestModel.Id);
                    report.Status = Status.Completed;
                    report.ReportPath = filePath;
                    await reportContext.SaveChangesAsync();

                    //todo:notification with signalR

                    _channel.BasicAck(@event.DeliveryTag, false);
                }
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }



    }
}
