using System;
using MediatR;

namespace SmartCharge.Commands;

public class DeleteConnectorCommand : IRequest
{
    public Guid Id { get; set; }
}