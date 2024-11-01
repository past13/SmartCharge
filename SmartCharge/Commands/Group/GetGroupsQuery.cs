using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class GetGroupsQuery : IRequest<Result<IEnumerable<GroupEntity>>>
{
}