using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;

namespace SmartCharge.Commands.ChargeStation;

public class UpdateChargeStationCommand : IRequest<ChargeStationEntity>
{
    public string Name { get; set; }
    public Guid GroupId  { get; set; }
    public List<ConnectorRequest> Connectors; 
    
    public UpdateChargeStationCommand(string name, List<ConnectorRequest> connectors)
    {
        Name = name;
        Connectors = connectors;
    }
}
