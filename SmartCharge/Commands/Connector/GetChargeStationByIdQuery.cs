using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class GetConnectorByIdQuery: IRequest<Result<ConnectorEntity>>
{
    public Guid Id { get; }

    public GetConnectorByIdQuery(Guid id)
    {
        Id = id;
    }
}