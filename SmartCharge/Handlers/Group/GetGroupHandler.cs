using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class GetGroupHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository)
    : IRequestHandler<GetGroupByIdQuery, Result<GroupEntity>>
{
    public async Task<Result<GroupEntity>> Handle(GetGroupByIdQuery query, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await groupRepository.GetGroupById(query.Id);
            if (group is null)
            {
                throw new ArgumentException($"A Group with Id {query.Id} does not exist.");
            }
                
            group.IsValidForChange();
            
            await unitOfWork.CommitAsync();
            
            return Result<GroupEntity>.Success(group);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
    }
}