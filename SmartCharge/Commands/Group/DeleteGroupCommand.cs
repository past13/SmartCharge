using System;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class DeleteGroupCommand : IRequest<ApiResponse<GroupEntity>>
{
    public Guid Id { get; set; }
    public DeleteGroupCommand(Guid id)
    {
        Id = id;
    }
}