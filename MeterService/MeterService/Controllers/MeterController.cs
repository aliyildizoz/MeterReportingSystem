using MeterService.Data;
using MeterService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase
    {
        private readonly MeterContext _context;

        public MeterController(MeterContext context)
        {
            _context = context;
        }

        [HttpGet("{serialNumber}")]
        public async Task<ActionResult<MeterReading>> GetLastReading(string serialNumber)
        {
            var reading = await _context.MeterReadings
                .Where(m => m.SerialNumber == serialNumber)
                .OrderByDescending(m => m.ReadingTime)
                .FirstOrDefaultAsync();

            if (reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }

        [HttpPost]
        public async Task<ActionResult<MeterReading>> PostReading(Models.MeterReading reading)
        {
            reading.Id = Guid.NewGuid();
            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLastReading), new { serialNumber = reading.SerialNumber }, reading);
        }
    }
}
