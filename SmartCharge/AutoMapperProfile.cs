using AutoMapper;
using SmartCharge.Domain.Entities;
using SmartCharge.DTOs;
using SmartCharge.Repository;

namespace SmartCharge;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GroupEntity, GroupDTO>()
            .ForMember(dest => 
                dest.ChargeStations, opt => 
                opt.MapFrom(src => src.ChargeStations));
        
        CreateMap<ChargeStationEntity, ChargeStationDTO>()
            .ForMember(dest => dest.Connectors, 
                opt => 
                    opt.MapFrom(src => src.Connectors));

        CreateMap<ConnectorEntity, ConnectorDTO>();
    }
}