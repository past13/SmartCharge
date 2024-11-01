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

public class DeleteChargeStationHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<DeleteChargeStationCommand, Result<ChargeStationEntity>>
{
    public async Task<Result<ChargeStationEntity>> Handle(DeleteChargeStationCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var chargeStation = await chargeStationRepository.GetChargeStationById(command.Id);
            if (chargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with the Id {command.Id} does not exists.");
            }
            
            var group = await groupRepository.GetGroupById(command.GroupId);
            if (group is null)
            {
                throw new ArgumentException($"A Group with Id {command.GroupId} does not exists.");
            }

            chargeStation.UpdateRowState(RowState.PendingDelete);
            
            group.RemoveChargeStation(chargeStation);
            group.UpdateCapacity();
            
            await chargeStationRepository.DeleteChargeStationById(command.Id);
            
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            
            return Result<ChargeStationEntity>.Success(null);
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