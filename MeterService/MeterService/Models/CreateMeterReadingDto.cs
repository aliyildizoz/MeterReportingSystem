using System;
using System.ComponentModel.DataAnnotations;

namespace MeterService.Models
{
    public class CreateMeterReadingDto
    {

        [MaxLength(8)]
        public string SerialNumber { get; set; }
        public DateTime ReadingTime { get; set; }
        public double EndIndex { get; set; }
        public double Voltage { get; set; }
        public double Current { get; set; }
    }
}
