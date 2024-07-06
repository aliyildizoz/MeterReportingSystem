using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace MeterService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MeterReading,MeterReadingDTO>().ForMember(dest=>dest.Id,act=>act.MapFrom(src=>src.Id.ToString())).ForMember(dest => dest.ReadingTime, act => act.MapFrom(src => Timestamp.FromDateTime(src.ReadingTime.ToUniversalTime()))).ReverseMap();
        }
    }
}
