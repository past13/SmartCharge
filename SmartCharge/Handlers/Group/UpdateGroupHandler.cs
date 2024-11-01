using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, Result<GroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGroupRepository _groupRepository;
    public UpdateGroupHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGroupRepository groupRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<GroupDto>> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
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
        
            return Result<GroupDto>.Success(_mapper.Map<GroupDto>(group));
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupDto>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupDto>.Failure("The Group was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            
            return Result<GroupDto>.Failure(ex.Message);
        }
    }
}