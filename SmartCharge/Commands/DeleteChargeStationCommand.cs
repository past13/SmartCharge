using System;
using MediatR;

namespace SmartCharge.Commands;

public class DeleteChargeStationCommand : IRequest
{
    public Guid Id { get; set; }
}