using System;

namespace ReportService.Models
{
    public class RabbitMQReportRequestModel
    {
        public string SerialNumber { get; set; }
        public Guid Id { get; set; }
    }
}
