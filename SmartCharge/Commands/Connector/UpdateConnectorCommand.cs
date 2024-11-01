using System;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class UpdateConnectorCommand : IRequest<Result<ConnectorDto>>
{
    public Guid Id { get; set; }
    public Guid ChargeStationId  { get; set; }
    public string Name { get; set; }
    public int MaxCurrentInAmps { get; set; }
    
    public UpdateConnectorCommand(Guid id, Guid chargeStationId, string name, int maxCurrentInAmps)
    {
        Id = id;
        ChargeStationId = chargeStationId;
        Name = name;
        MaxCurrentInAmps = maxCurrentInAmps;
    }
}
