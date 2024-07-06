using Grpc.Core;
using MeterService;

namespace ReportService.gRPC.Services
{
    public interface IMeterGrpcService
    {
        AsyncUnaryCall<MeterReadingResponse> GetMetersBySerialNumberAsync(MeterReadingRequest request);
    }
}
