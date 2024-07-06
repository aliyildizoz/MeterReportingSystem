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

namespace ReportService.RabbitMQ.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ReportContext _reportContext;
        private readonly IMeterGrpcService _metergGrpcService;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, RabbitMQClientService rabbitMQClientService, ReportContext reportContext, IMeterGrpcService metergGrpcService)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _reportContext = reportContext;
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

                var requestModel = JsonSerializer.Deserialize<RabbitMQReportRequestModel>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                var response = await _metergGrpcService.GetMetersBySerialNumberAsync(new MeterReadingRequest() { SerialNumber = requestModel.SerialNumber });

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var filePath = @$"{Environment.CurrentDirectory}/wwwroot/reports/{requestModel.SerialNumber}.xlsx";
                var file = new FileInfo(filePath);
                using ExcelPackage excel = new ExcelPackage(file);

                var workSheet = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == $"Meter Report({requestModel.SerialNumber})");
                if (workSheet == null) workSheet = excel.Workbook.Worksheets.Add($"Meter Report({requestModel.SerialNumber})");


                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                workSheet.Cells[1, 1].Value = "Number";
                workSheet.Cells[1, 2].Value = "Id";
                workSheet.Cells[1, 3].Value = "Serial Number";
                workSheet.Cells[1, 4].Value = "Reading Time";
                workSheet.Cells[1, 5].Value = "End Index";
                workSheet.Cells[1, 6].Value = "Voltage";
                workSheet.Cells[1, 7].Value = "Current";


                int recordIndex = 2;

                foreach (var item in response.MeterReadingDtos)
                {
                    if (item != null)
                    {
                        workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                        workSheet.Cells[recordIndex, 2].Value = item.Id;
                        workSheet.Cells[recordIndex, 3].Value = item.SerialNumber;
                        workSheet.Cells[recordIndex, 4].Value = item.ReadingTime.ToDateTime().ToString("d");
                        workSheet.Cells[recordIndex, 5].Value = item.EndIndex;
                        workSheet.Cells[recordIndex, 6].Value = item.Voltage;
                        workSheet.Cells[recordIndex, 7].Value = item.Current;
                        recordIndex++;
                    }
                }
                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                workSheet.Column(6).AutoFit();
                workSheet.Column(7).AutoFit();

                if (File.Exists(filePath)) File.Delete(filePath);

                FileStream objFileStrm = File.Create(filePath);
                objFileStrm.Close();

                File.WriteAllBytes(filePath, excel.GetAsByteArray());
                excel.Dispose();

                var report = await _reportContext.ReportRequests.FirstOrDefaultAsync(x => x.Id == requestModel.Id);
                report.Status = Status.Completed;
                report.ReportPath = filePath;
                await _reportContext.SaveChangesAsync();

                //todo:notification with signalR

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }



    }
}
