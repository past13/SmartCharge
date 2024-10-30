using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;
using SmartCharge.Requests;

namespace SmartCharge.Commands;

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
