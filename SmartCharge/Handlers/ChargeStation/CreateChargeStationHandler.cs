using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.ChargeStation;

public class CreateChargeStationHandler(
    IUnitOfWork unitOfWork,
    IChargeStationRepository chargeStationRepository,
    IGroupRepository groupRepository)
    : IRequestHandler<CreateChargeStationCommand, Result<ChargeStationEntity>>
{
    public async Task<Result<ChargeStationEntity>> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await groupRepository.GetGroupById(command.GroupId);
            if (group == null)
            {
                throw new ArgumentException($"A Group with Id {command.GroupId} does not exists.");
            }

            var chargeStationName = command.Name.Trim();
            var chargeStationNameExist = await chargeStationRepository.IsNameExist(chargeStationName);
            if (chargeStationNameExist)
            {
                throw new ArgumentException($"A ChargeStation with the name {chargeStationName} already exists.");
            }
        
            var chargeStation = ChargeStationEntity.Create(chargeStationName);

            if (command.Connectors is null || command.Connectors.Count is 0)
            {
                throw new ArgumentException($"A ChargeStation name {chargeStationName} do not have connector.");
            } 
            
            foreach (var connectorRequest in command.Connectors)
            {
                var connector = ConnectorEntity.Create(connectorRequest.Name, connectorRequest.MaxCapacityInAmps);
                chargeStation.AddConnector(connector);
            }
            
            group.AddChargeStation(chargeStation);
            group.UpdateCapacity();
            
            await chargeStationRepository.AddChargeStation(chargeStation);

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
            
            return Result<ChargeStationEntity>.Success(chargeStation);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
    }
}