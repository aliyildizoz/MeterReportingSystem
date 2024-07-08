using AutoMapper;
using MeterService.Controllers;
using MeterService.Data;
using MeterService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeterServiceTests.Controllers
{
    public class MeterControllerTests
    {
        private readonly MeterContext _context;
        private readonly IMapper _mapper;
        private readonly MeterController _controller;

        public MeterControllerTests()
        {
            var options = new DbContextOptionsBuilder<MeterContext>()
                .UseInMemoryDatabase(databaseName: "TestMeterDatabase")
                .Options;
            _context = new MeterContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateMeterReadingDto, MeterReading>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            SeedData(_context);

            _controller = new MeterController(_context, _mapper);
        }

        private void SeedData(MeterContext context)
        {
            context.MeterReadings.AddRange(
                new MeterReading { Id = Guid.NewGuid(), SerialNumber = "12345", ReadingTime = DateTime.Now.AddDays(-1), Current = 100, EndIndex=50, Voltage=30 },
                new MeterReading { Id = Guid.NewGuid(), SerialNumber = "12345", ReadingTime = DateTime.Now, Current = 100, EndIndex = 50, Voltage = 30 },
                new MeterReading { Id = Guid.NewGuid(), SerialNumber = "67890", ReadingTime = DateTime.Now.AddDays(-2), Current = 100, EndIndex = 50, Voltage = 30 }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task GetLastReading_ReturnsCorrectReading()
        {
            var result = await _controller.GetLastReading("12345");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsCorrectReading()
        {
            var existingReading = _context.MeterReadings.First();

            var result = await _controller.GetById(existingReading.Id.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reading = Assert.IsType<MeterReading>(okResult.Value);
            Assert.Equal(existingReading.Id, reading.Id);
        }

        [Fact]
        public async Task GetAll_ReturnsAllReadings()
        {
            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var readings = Assert.IsType<List<MeterReading>>(okResult.Value);
            Assert.Equal(3, readings.Count);
        }

        [Fact]
        public async Task PostReading_CreatesNewReading()
        {
            var newReadingDto = new CreateMeterReadingDto
            {
                SerialNumber = "12345",
                ReadingTime = DateTime.Now,
                Current = 100,
                EndIndex = 50,
                Voltage = 30
            };

            var result = await _controller.PostReading(newReadingDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var reading = Assert.IsType<MeterReading>(createdAtActionResult.Value);
            Assert.NotEmpty(reading.Id.ToString());
        }
    }
}
