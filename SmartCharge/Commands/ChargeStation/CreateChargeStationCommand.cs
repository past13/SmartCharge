using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests.Connector;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.ChargeStation;

public class CreateChargeStationCommand(Guid groupId, string name, List<ConnectorRequest> connectors)
    : IRequest<Result<ChargeStationEntity>>
{
    public Guid GroupId { get; set; } = groupId;
    public string Name { get; set; } = name;

    public List<ConnectorRequest> Connectors = connectors;
}