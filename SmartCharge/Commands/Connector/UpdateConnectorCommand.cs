using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class UpdateConnectorCommand(Guid id, Guid chargeStationId, string name, int maxCurrentInAmps)
    : IRequest<Result<ConnectorEntity>>
{
    public Guid Id { get; set; } = id;
    public Guid ChargeStationId  { get; set; } = chargeStationId;
    public string Name { get; set; } = name;
    public int MaxCurrentInAmps { get; set; } = maxCurrentInAmps;
}
