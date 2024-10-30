using System;
using MediatR;

namespace SmartCharge.Commands.Connector;

public class DeleteConnectorCommand : IRequest
{
    public Guid Id { get; set; }
}