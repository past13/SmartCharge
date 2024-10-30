using AutoMapper;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GroupEntity, GroupDto>()
            .ForMember(dest => 
                dest.ChargeStations, opt => 
                opt.MapFrom(src => src.ChargeStations));
        
        CreateMap<ChargeStationEntity, ChargeStationDto>()
            .ForMember(dest => dest.Connectors, 
                opt => 
                    opt.MapFrom(src => src.Connectors));

        CreateMap<ConnectorEntity, ConnectorDto>();
    }
}