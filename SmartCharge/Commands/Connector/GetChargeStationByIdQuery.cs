using System;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class GetConnectorByIdQuery: IRequest<Result<ConnectorDto>>
{
    public Guid Id { get; }

    public GetConnectorByIdQuery(Guid id)
    {
        Id = id;
    }
}