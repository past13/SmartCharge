using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class DeleteGroupCommand(Guid id) : IRequest<Result<GroupEntity>>
{
    public Guid Id { get; set; } = id;
}