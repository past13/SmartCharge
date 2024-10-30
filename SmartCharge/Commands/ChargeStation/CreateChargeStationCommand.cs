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
    public int MaxCurrentInAmps { get; set; }

    public List<ConnectorRequest> Connectors { get; set; } 
    
    public CreateChargeStationCommand(string name, int maxCurrentInAmps, List<ConnectorRequest> connectors)
    {
        Name = name;
        MaxCurrentInAmps = maxCurrentInAmps;
        Connectors = connectors;
    }
}