using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;
using SmartCharge.Requests;

namespace SmartCharge.Commands;

public class UpdateConnectorCommand : IRequest<ConnectorEntity>
{
    public string Name { get; set; }
    public Guid ChargeStationId  { get; set; }
    
    public UpdateConnectorCommand(string name)
    {
        Name = name;
    }
}
