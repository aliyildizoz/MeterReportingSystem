using ReportService.Data;
using ReportService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportService.RabbitMQ.Services;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportContext _context;
        private readonly ILogger<ReportController> _logger;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        public ReportController(ILogger<ReportController> logger, ReportContext context, IRabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _logger = logger;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportRequest>>> GetReportRequests()
        {
            return await _context.ReportRequests.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportRequest>> GetReportRequest(string id)
        {
            var reportRequest = await _context.ReportRequests.FirstOrDefaultAsync(r => r.Id == Guid.Parse(id));

            if (reportRequest == null)
            {
                return NotFound();
            }

            return Ok(reportRequest);
        }

        [HttpGet("Download/{id}")]
        public async Task<ActionResult<ReportRequest>> Download(string id)
        {
            var reportRequest = await _context.ReportRequests.FirstOrDefaultAsync(r => r.Id == Guid.Parse(id));

            if (!System.IO.File.Exists(reportRequest.ReportPath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(reportRequest.ReportPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportRequest.SerialNumber);
        }

        [HttpPost]
        public async Task<ActionResult<ReportRequest>> CreateReportRequest([FromBody] string serialNumber)
        {
            var reportRequest = new ReportRequest
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.UtcNow,
                Status = Status.Preparing,
                ReportPath = string.Empty,
                SerialNumber = serialNumber
            };

            _context.ReportRequests.Add(reportRequest);
            await _context.SaveChangesAsync();

            _rabbitMQPublisher.Publish(new RabbitMQReportRequestModel() { Id = reportRequest.Id, SerialNumber = serialNumber });

            return CreatedAtAction(nameof(GetReportRequest), new { id = reportRequest.Id }, reportRequest);
        }

    }
}

