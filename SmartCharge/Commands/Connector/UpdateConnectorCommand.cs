using System;
using MediatR;
using SmartCharge.Domain.Entities;

namespace SmartCharge.Commands.Connector;

public class UpdateConnectorCommand : IRequest<ConnectorEntity>
{
    public string Name { get; set; }
    public Guid ChargeStationId  { get; set; }
    
    public UpdateConnectorCommand(string name)
    {
        Name = name;
    }
}
