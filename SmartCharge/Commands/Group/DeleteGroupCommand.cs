using System;
using MediatR;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class DeleteGroupCommand : IRequest<ApiResponse<Unit>>
{
    public Guid Id { get; set; }
}