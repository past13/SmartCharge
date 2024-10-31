using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class DeleteChargeStationCommand : IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public DeleteChargeStationCommand(Guid id, Guid groupId)
    {
        Id = id;
        GroupId = groupId;
    }
}