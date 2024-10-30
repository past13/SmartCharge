using System;
using MediatR;

namespace SmartCharge.Commands.ChargeStation;

public class DeleteChargeStationCommand : IRequest
{
    public Guid Id { get; set; }
}