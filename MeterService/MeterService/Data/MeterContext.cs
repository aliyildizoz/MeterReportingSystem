using MeterService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MeterService.Data
{
    public class MeterContext : DbContext
    {
        public MeterContext(DbContextOptions<MeterContext> options) : base(options)
        {
        }

        public DbSet<Models.MeterReading> MeterReadings { get; set; }
    }
}
