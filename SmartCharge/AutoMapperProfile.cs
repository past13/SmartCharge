using AutoMapper;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;

namespace SmartCharge;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GroupEntity, GroupDto>();
        CreateMap<ChargeStationEntity, ChargeStationDto>();
        CreateMap<ConnectorEntity, ConnectorDto>();
    }
}