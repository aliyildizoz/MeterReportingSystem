using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Controllers;
using ReportService.Data;
using ReportService.Models;
using ReportService.RabbitMQ.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReportService.Tests
{
    public class ReportControllerTests
    {
        private readonly ReportContext _context;
        private readonly Mock<ILogger<ReportController>> _loggerMock;
        private readonly Mock<IRabbitMQPublisher> _rabbitMQPublisherMock;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            var options = new DbContextOptionsBuilder<ReportContext>()
                .UseInMemoryDatabase(databaseName: "TestReportDatabase")
                .Options;
            _context = new ReportContext(options);

            _loggerMock = new Mock<ILogger<ReportController>>();
            _rabbitMQPublisherMock = new Mock<IRabbitMQPublisher>();

            SeedData(_context);

            _controller = new ReportController(_loggerMock.Object, _context, _rabbitMQPublisherMock.Object);
        }

        private void SeedData(ReportContext context)
        {
            context.ReportRequests.AddRange(
                new ReportRequest { Id = Guid.NewGuid(), RequestDate = DateTime.UtcNow.AddDays(-1), Status = Status.Completed, ReportPath = "wwwroot/report/report1.xlsx", SerialNumber = "12345" },
                new ReportRequest { Id = Guid.NewGuid(), RequestDate = DateTime.UtcNow, Status = Status.Preparing, ReportPath = "", SerialNumber = "67890" }
            );
            context.SaveChanges();
        }

        [Fact]
        public async Task GetReportRequests_ReturnsAllReportRequests()
        {
            _context.ReportRequests.RemoveRange(_context.ReportRequests);
            _context.SaveChanges();
            SeedData(_context);
            var result = await _controller.GetReportRequests();

            var reportRequests = Assert.IsType<List<ReportRequest>>(result.Value);
            Assert.Equal(2, reportRequests.Count);
        }

        [Fact]
        public async Task GetReportRequest_ReturnsReportRequest()
        {
            var existingRequest = _context.ReportRequests.First();

            var result = await _controller.GetReportRequest(existingRequest.Id.ToString());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var reportRequest = Assert.IsType<ReportRequest>(okResult.Value);
            Assert.Equal(existingRequest.Id, reportRequest.Id);
        }

        [Fact]
        public async Task Download_ReturnsNotFound_WhenFileDoesNotExist()
        {
            var existingRequest = _context.ReportRequests.First(r => r.Status == Status.Preparing);

            var result = await _controller.Download(existingRequest.Id.ToString());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateReportRequest_CreatesAndPublishesRequest()
        {
            var serialNumber = "12345";

            var result = await _controller.CreateReportRequest(serialNumber);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var reportRequest = Assert.IsType<ReportRequest>(createdAtActionResult.Value);
            Assert.Equal(serialNumber, reportRequest.SerialNumber);
            Assert.Equal(Status.Preparing, reportRequest.Status);

            _rabbitMQPublisherMock.Verify(r => r.Publish(It.IsAny<RabbitMQReportRequestModel>()), Times.Once);
        }
    }
}
