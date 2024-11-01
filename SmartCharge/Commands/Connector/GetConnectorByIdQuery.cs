using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Connector;

public class GetConnectorByIdQuery(Guid id) : IRequest<Result<ConnectorEntity>>
{
    public Guid Id { get; } = id;
}