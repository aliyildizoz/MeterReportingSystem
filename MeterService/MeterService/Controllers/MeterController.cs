using AutoMapper;
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
        private readonly IMapper _mapper;


        public MeterController(MeterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetLastReading/{serialNumber}")]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<MeterReading>> GetById(string id)
        {
            var reading = await _context.MeterReadings
                .Where(m => m.Id.ToString() == id)
                .FirstOrDefaultAsync();

            if (reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }

        [HttpGet]
        public async Task<ActionResult<List<MeterReading>>> GetAll()
        {
            var reading = await _context.MeterReadings
                .OrderByDescending(m => m.ReadingTime).ToListAsync();

            if (reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }


        [HttpPost]
        public async Task<ActionResult<MeterReading>> PostReading([FromBody] Models.CreateMeterReadingDto readingDto)
        {
            var reading = _mapper.Map<MeterReading>(readingDto);
            reading.Id = Guid.NewGuid();
            _context.MeterReadings.Add(reading);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLastReading), new { serialNumber = reading.SerialNumber }, reading);
        }
    }
}
