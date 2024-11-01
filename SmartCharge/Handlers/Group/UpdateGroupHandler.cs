using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, Result<GroupEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    public UpdateGroupHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository)
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<GroupEntity>> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var groupName = command.Name.Trim();
            var groupNameExist = await _groupRepository.IsNameExist(groupName, command.Id);
            if (groupNameExist)
            {
                throw new ArgumentException($"A Group with the name {groupName} already exists.");
            }
        
            var group = await _groupRepository.GetGroupById(command.Id);
            if (group is null)
            {
                throw new ArgumentException($"A Group does not exists.");
            }

            group.IsValidForChange();

            group.Update(groupName);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        
            return Result<GroupEntity>.Success(group);
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