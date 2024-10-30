using System;
using MediatR;

namespace SmartCharge.Commands.Group;

public class DeleteGroupCommand : IRequest
{
    public Guid Id { get; set; }
}