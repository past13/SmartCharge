using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class UpdateChargeStationCommand(Guid id, Guid groupId, string name) : IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; set; } = id;
    public Guid GroupId  { get; set; } = groupId;
    public string Name { get; set; } = name;
}
