using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class DeleteConnectorCommand : IRequest<Result<ConnectorEntity>>
{
    public Guid Id { get; set; }
    public Guid ChargeStationId { get; set; }
    public DeleteConnectorCommand(Guid chargeStationId, Guid id)
    {
        Id = id;
        ChargeStationId = chargeStationId;
    }
}