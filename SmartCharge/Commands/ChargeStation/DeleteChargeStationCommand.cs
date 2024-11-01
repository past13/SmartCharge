using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class DeleteChargeStationCommand(Guid id, Guid groupId) : IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; set; } = id;
    public Guid GroupId { get; set; } = groupId;
}