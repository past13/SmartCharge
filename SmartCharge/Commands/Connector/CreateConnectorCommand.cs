using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;

namespace SmartCharge.Commands.Connector;

public class CreateConnectorCommand : IRequest<ConnectorEntity>
{
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    
    public Guid ChargeStationId { get; set; }
    
    public CreateConnectorCommand(string name, int capacityInAmps, ChargeStationRequest chargeStation)
    {
        Name = name;
        CapacityInAmps = capacityInAmps;
        //Todo: missing chargeStation
    }
}
