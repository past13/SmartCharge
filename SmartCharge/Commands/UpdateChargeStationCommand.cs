using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;
using SmartCharge.Requests;

namespace SmartCharge.Commands;

public class UpdateChargeStationCommand : IRequest<ChargeStationEntity>
{
    public string Name { get; set; }
    public Guid GroupId  { get; set; }
    public List<ConnectorRequest> Connectors { get; set; } 
    
    public UpdateChargeStationCommand(string name, List<ConnectorRequest> connectors)
    {
        Name = name;
        Connectors = connectors;
    }
}
