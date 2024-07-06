using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
using MeterService.Data;
using MeterService.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterService.Services
{
    public class MeterGRPCService: MetergRPCService.MetergRPCServiceBase
    {
        private readonly MeterContext _context;
        private readonly IMapper _mapper;

        public MeterGRPCService(MeterContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async override Task<MeterReadingResponse> GetMetersBySerialNumber(MeterReadingRequest request, ServerCallContext context)
        {
            var readings = await _context.MeterReadings
                 .Where(m => m.SerialNumber == request.SerialNumber)
                 .OrderByDescending(m => m.ReadingTime).ToListAsync();

            var res = readings.Select(_mapper.Map<MeterReadingDTO>).ToList();
            //var res = _mapper.Map<List<MeterReading>, List<MeterReadingDTO>>(readings);

            var response =  new MeterReadingResponse();
            response.MeterReadingDTOs.AddRange(res);

            return response;
        }
    }
}
