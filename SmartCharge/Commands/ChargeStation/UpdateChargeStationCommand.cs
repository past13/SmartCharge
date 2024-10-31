using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class UpdateChargeStationCommand : IRequest<Result<ChargeStationEntity>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid GroupId  { get; set; }
    public List<ConnectorRequest> Connectors; 
    
    public UpdateChargeStationCommand(Guid id, Guid groupId, string name, List<ConnectorRequest> connectors)
    {
        Id = id;
        Name = name;
        Connectors = connectors;
        GroupId = groupId;
    }
}
