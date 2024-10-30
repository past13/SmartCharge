using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class CreateChargeStationCommand: IRequest<ApiResponse<ChargeStationEntity>>
{
    public Guid GroupId { get; set; }
    public string Name { get; set; }

    public List<ConnectorRequest> Connectors = new List<ConnectorRequest>(); 
    
    public CreateChargeStationCommand(Guid groupId, string name, List<ConnectorRequest> connectors)
    {
        GroupId = groupId;
        Name = name;
        Connectors = connectors;
    }
}