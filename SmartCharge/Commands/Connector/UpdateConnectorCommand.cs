using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class UpdateConnectorCommand : IRequest<ApiResponse<ConnectorEntity>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ChargeStationId  { get; set; }
    
    public UpdateConnectorCommand(Guid id, Guid chargeStationId, string name)
    {
        Id = id;
        ChargeStationId = chargeStationId;
        Name = name;
    }
}
