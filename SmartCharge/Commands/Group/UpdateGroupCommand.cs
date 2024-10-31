using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class UpdateGroupCommand : IRequest<Result<GroupEntity>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public UpdateGroupCommand(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
