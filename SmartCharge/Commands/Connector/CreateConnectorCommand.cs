using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class CreateConnectorCommand(string name, int capacityInAmps, Guid chargeStationId)
    : IRequest<Result<ConnectorEntity>>
{
    public string Name { get; set; } = name;
    public int CapacityInAmps { get; set; } = capacityInAmps;

    public Guid ChargeStationId { get; set; } = chargeStationId;
}
