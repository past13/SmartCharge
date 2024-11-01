using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class CreateConnectorCommand : IRequest<Result<ConnectorEntity>>
{
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    
    public Guid ChargeStationId { get; set; }
    
    public CreateConnectorCommand(string name, int capacityInAmps, Guid chargeStationId)
    {
        Name = name;
        CapacityInAmps = capacityInAmps;
        ChargeStationId = chargeStationId;
    }
}
