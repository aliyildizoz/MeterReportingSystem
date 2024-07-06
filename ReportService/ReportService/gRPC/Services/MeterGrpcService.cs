using Grpc.Core;
using MeterService;
using static MeterService.MeterGrpcService;

namespace ReportService.gRPC.Services
{
    public class MeterGrpcService : IMeterGrpcService
    {
        private readonly MeterGrpcServiceClient _meterGrpcService;
        public MeterGrpcService(MeterGrpcServiceClient meterGrpcService)
        {
            _meterGrpcService = meterGrpcService;
        }
        public AsyncUnaryCall<MeterReadingResponse> GetMetersBySerialNumberAsync(MeterReadingRequest request)
        {
            return _meterGrpcService.GetMetersBySerialNumberAsync(request);
        }
    }
}
