using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class UpdateGroupCommand(Guid id, string name) : IRequest<Result<GroupEntity>>
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
}
