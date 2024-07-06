using ReportService.Models;
using Microsoft.EntityFrameworkCore;

namespace ReportService.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options) : base(options)
        {
        }

        public DbSet<ReportRequest> ReportRequests { get; set; }
    }
}
