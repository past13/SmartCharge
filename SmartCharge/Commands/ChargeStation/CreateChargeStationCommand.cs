using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Requests.Connector;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class CreateChargeStationCommand: IRequest<Result<ChargeStationDto>>
{
    public Guid GroupId { get; set; }
    public string Name { get; set; }

    public List<ConnectorRequest> Connectors; 
    
    public CreateChargeStationCommand(Guid groupId, string name, List<ConnectorRequest> connectors)
    {
        GroupId = groupId;
        Name = name;
        Connectors = connectors;
    }
}