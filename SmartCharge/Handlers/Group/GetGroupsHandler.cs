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

public class GetGroupsHandler : IRequestHandler<GetGroupsQuery, Result<IEnumerable<GroupEntity>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    
    public GetGroupsHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository
        )
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<IEnumerable<GroupEntity>>> Handle(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var groups = await _groupRepository.GetGroups();
            
            await _unitOfWork.CommitAsync();
            
            return Result<IEnumerable<GroupEntity>>.Success(groups);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<GroupEntity>>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<IEnumerable<GroupEntity>>.Failure(ex.Message);
        }
    }
}