using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class DeleteChargeStationCommand : IRequest<ApiResponse<ChargeStationEntity>>
{
    public Guid Id { get; set; }
    public DeleteChargeStationCommand(Guid id)
    {
        Id = id;
    }
}