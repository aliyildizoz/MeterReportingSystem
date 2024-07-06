using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
using MeterService.Data;
using MeterService.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterService.gRPC.Services
{
    public class MeterGrpcService : MeterService.MeterGrpcService.MeterGrpcServiceBase
    {
        private readonly MeterContext _context;
        private readonly IMapper _mapper;

        public MeterGrpcService(MeterContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async override Task<MeterReadingResponse> GetMetersBySerialNumber(MeterReadingRequest request, ServerCallContext context)
        {
            var readings = await _context.MeterReadings
                 .Where(m => m.SerialNumber == request.SerialNumber)
                 .OrderByDescending(m => m.ReadingTime).ToListAsync();

            var res = readings.Select(_mapper.Map<MeterReadingDto>).ToList();
            var response = new MeterReadingResponse();
            response.MeterReadingDtos.AddRange(res);

            return response;
        }

    }
}
