using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class UpdateChargeStationHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<UpdateChargeStationCommand, Result<ChargeStationEntity>>
{
    public async Task<Result<ChargeStationEntity>> Handle(UpdateChargeStationCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStationName = command.Name.Trim();
            var chargeStationNameExist = await chargeStationRepository.IsNameExist(chargeStationName, command.Id);
            if (chargeStationNameExist)
            {
                throw new ArgumentException($"A ChargeStation with the name {chargeStationName} already exists.");
            }
        
            var chargeStation = await chargeStationRepository.GetChargeStationById(command.Id);
            if (chargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.Id} does not exists.");
            }
            
            var newGroup = await groupRepository.GetGroupById(command.GroupId);
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

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
        
            return Result<ChargeStationEntity>.Success(chargeStation);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure("The ChargeStation was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
    }
}