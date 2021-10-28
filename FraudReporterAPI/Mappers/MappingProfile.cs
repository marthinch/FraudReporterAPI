using AutoMapper;
using FraudReporterAPI.DTOs;
using FraudReporterAPI.Helpers;
using FraudReporterAPI.Models;

namespace FraudReporterAPI.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            FraudMapping();
        }

        private void FraudMapping()
        {
            CreateMap<FraudDTO, Fraud>();

            CreateMap<Fraud, FraudListDTO>()
               .ForMember(destination => destination.Status, o => o.MapFrom(source => StatusHelper.ConvertStatus(source.Status)));

            CreateMap<Fraud, FraudDetailDTO>()
                .ForMember(destination => destination.Status, o => o.MapFrom(source => StatusHelper.ConvertStatus(source.Status)));
        }
    }
}
