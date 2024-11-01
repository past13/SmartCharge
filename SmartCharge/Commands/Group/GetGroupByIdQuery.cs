using System;
using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class GetGroupByIdQuery : IRequest<Result<GroupDto>>
{
    public Guid Id { get; }

    public GetGroupByIdQuery(Guid id)
    {
        Id = id;
    }
}