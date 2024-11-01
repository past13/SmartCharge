using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class DeleteConnectorCommand(Guid chargeStationId, Guid id) : IRequest<Result<ConnectorEntity>>
{
    public Guid Id { get; set; } = id;
    public Guid ChargeStationId { get; set; } = chargeStationId;
}