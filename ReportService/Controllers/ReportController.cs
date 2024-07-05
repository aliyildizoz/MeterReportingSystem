using ReportService.Data;
using ReportService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportContext _context;
        private readonly ILogger<ReportController> _logger;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        public ReportController(ILogger<ReportController> logger, ReportContext context, RabbitMQPublisher rabbitMQPublisher)
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
        public async Task<ActionResult<ReportRequest>> GetReportRequest(Guid id)
        {
            var reportRequest = await _context.ReportRequests.FindAsync(id);

            if (reportRequest == null)
            {
                return NotFound();
            }

            return Ok(reportRequest);
        }

        [HttpPost]
        public async Task<ActionResult<ReportRequest>> CreateReportRequest()
        {
            var reportRequest = new ReportRequest
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.UtcNow,
                Status = Status.Preparing,
                ReportPath = string.Empty // Initial content, will be filled when report is ready
            };

            _context.ReportRequests.Add(reportRequest);
            await _context.SaveChangesAsync();

            _rabbitMQPublisher.Publish(new RabbitMQReportRequestModel() { Id = reportRequest.Id });

            return CreatedAtAction(nameof(GetReportRequest), new { id = reportRequest.Id }, reportRequest);
        }

    }
}

