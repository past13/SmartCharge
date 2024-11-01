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

public class CreateGroupHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IChargeStationRepository chargeStationRepository)
    : IRequestHandler<CreateGroupCommand, Result<GroupEntity>>
{
    public async Task<Result<GroupEntity>> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();
        
        try
        {
            var groupName = command.Name.Trim();

            var groupNameExist = await groupRepository.IsNameExist(groupName);
            if (groupNameExist)
            {
                throw new ArgumentException($"A Group with the name {groupName} already exists.");
            }

            var group = GroupEntity.Create(groupName);

            if (command.ChargeStation is not null)
            {
                var chargeStationName = command.ChargeStation.Name.Trim();
                var chargeStationExist = await chargeStationRepository.IsNameExist(chargeStationName);
                if (chargeStationExist)
                {
                    throw new ArgumentException($"A ChargeStation with the name {chargeStationName} already exists.");
                }

                var chargeStation = ChargeStationEntity.Create(chargeStationName);
                group.AddChargeStation(chargeStation);

                if (command.ChargeStation.Connectors is null || command.ChargeStation.Connectors.Count is 0)
                {
                    throw new ArgumentException($"A ChargeStation name {chargeStationName} do not have connector.");
                }
                
                foreach (var connectorRequest in command.ChargeStation.Connectors)
                {
                    var connector = ConnectorEntity.Create(connectorRequest.Name, connectorRequest.MaxCapacityInAmps);
                    chargeStation.AddConnector(connector);
                }

                group.UpdateCapacity();
            }

            await groupRepository.AddGroup(group);

            await unitOfWork.SaveChangesAsync();
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