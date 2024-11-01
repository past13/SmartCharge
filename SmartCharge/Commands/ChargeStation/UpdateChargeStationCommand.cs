using System;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class UpdateChargeStationCommand : IRequest<Result<ChargeStationDto>>
{
    public Guid Id { get; set; }
    public Guid GroupId  { get; set; }
    public string Name { get; set; }
    
    public UpdateChargeStationCommand(Guid id, Guid groupId, string name)
    {
        Id = id;
        GroupId = groupId;
        Name = name;
    }
}
