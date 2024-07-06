using System;
using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    public class ReportRequest
    {
        public Guid Id { get; set; }
        [MaxLength(8)]
        public string SerialNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public Status Status { get; set; }
        public string? ReportPath { get; set; } 
    }

    public enum Status
    {
        Preparing,
        Completed
    }
}
