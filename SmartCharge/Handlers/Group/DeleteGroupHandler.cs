using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class DeleteGroupHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository)
    : IRequestHandler<DeleteGroupCommand, Result<GroupEntity>>
{
    public async Task<Result<GroupEntity>> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await groupRepository.GetGroupById(command.Id);
            if (group is null)
            {
                throw new ArgumentException($"A Group with the Id {command.Id} does not exists.");
            }

            group.UpdateRowState(RowState.PendingDelete);
            
            await groupRepository.DeleteGroupById(command.Id);

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            
            return Result<GroupEntity>.Success(null);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure("The Group was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
    }
}