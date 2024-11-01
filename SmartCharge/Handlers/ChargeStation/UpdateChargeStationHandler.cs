using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class UpdateChargeStationHandler : IRequestHandler<UpdateChargeStationCommand, Result<ChargeStationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    public UpdateChargeStationHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository
        )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<ChargeStationDto>> Handle(UpdateChargeStationCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStationName = command.Name.Trim();
            var chargeStationNameExist = await _chargeStationRepository.IsNameExist(chargeStationName, command.Id);
            if (chargeStationNameExist)
            {
                throw new ArgumentException($"A ChargeStation with the name {chargeStationName} already exists.");
            }
        
            var chargeStation = await _chargeStationRepository.GetChargeStationById(command.Id);
            if (chargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.Id} does not exists.");
            }
            
            var newGroup = await _groupRepository.GetGroupById(command.GroupId);
            if (newGroup is null)
            {
                throw new ArgumentException($"A Group with Id {command.GroupId} does not exists.");
            }
            
            chargeStation.IsValidForChange();

            if (chargeStation.GroupId != newGroup.Id)
            {
                chargeStation.GroupEntity.RemoveChargeStation(chargeStation);
                newGroup.AddChargeStation(chargeStation);
                
                chargeStation.GroupEntity.UpdateCapacity();
                newGroup.UpdateCapacity();
            }
            
            chargeStation.UpdateName(chargeStationName);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        
            return Result<ChargeStationDto>.Success(_mapper.Map<ChargeStationDto>(chargeStation));
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationDto>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationDto>.Failure("The ChargeStation was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationDto>.Failure(ex.Message);
        }
    }
}