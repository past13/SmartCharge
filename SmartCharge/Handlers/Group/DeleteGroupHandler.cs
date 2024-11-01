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

public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Result<GroupEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    public DeleteGroupHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository
        )
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<GroupEntity>> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await _groupRepository.GetGroupById(command.Id);
            if (group is null)
            {
                throw new ArgumentException($"A Group with the Id {command.Id} does not exists.");
            }

            group.UpdateStateDelete(RowState.PendingDelete);
            
            await _groupRepository.DeleteGroupById(command.Id);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            
            return Result<GroupEntity>.Success(null);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure("The Group was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupEntity>.Failure(ex.Message);
        }
    }
}