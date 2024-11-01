using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class GetGroupsHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository)
    : IRequestHandler<GetGroupsQuery, Result<IEnumerable<GroupEntity>>>
{
    public async Task<Result<IEnumerable<GroupEntity>>> Handle(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var groups = await groupRepository.GetGroups();
            
            await unitOfWork.CommitAsync();
            
            return Result<IEnumerable<GroupEntity>>.Success(groups);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<GroupEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<IEnumerable<GroupEntity>>.Failure(ex.Message);
        }
    }
}