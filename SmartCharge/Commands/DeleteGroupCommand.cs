using System;
using MediatR;

namespace SmartCharge.Commands;

public class DeleteGroupCommand : IRequest
{
    public Guid Id { get; set; }
}