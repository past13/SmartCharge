using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Group;

public class GetGroupHandler : IRequestHandler<GetGroupByIdQuery, Result<GroupDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;
    
    public GetGroupHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<GroupDto>> Handle(GetGroupByIdQuery query, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await _groupRepository.GetGroupById(query.Id);
            if (group is null)
            {
                throw new ArgumentException($"A Group with Id {query.Id} does not exist.");
            }
                
            group.IsValidForChange();
            
            await _unitOfWork.CommitAsync();
            
            return Result<GroupDto>.Success(_mapper.Map<GroupDto>(group));
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<GroupDto>.Failure(ex.Message);
        }
    }
}