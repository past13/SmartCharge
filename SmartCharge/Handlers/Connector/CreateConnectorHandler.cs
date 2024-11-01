using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class CreateConnectorHandler(
    IUnitOfWork unitOfWork,
    IGroupRepository groupRepository,
    IChargeStationRepository chargeStationRepository,
    IConnectorRepository connectorRepository)
    : IRequestHandler<CreateConnectorCommand, Result<ConnectorEntity>>
{
    public async Task<Result<ConnectorEntity>> Handle(CreateConnectorCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync();

        try
        {
            var connectorName = command.Name.Trim();
            var connectorNameExist = await connectorRepository.IsNameExist(connectorName);
            if (connectorNameExist)
            {
                throw new ArgumentException($"A Connector with the name {connectorName} already exists.");
            }
            
            var isChargeStationExist = await chargeStationRepository.IsChargeStationExist(command.ChargeStationId);
            if (!isChargeStationExist)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.ChargeStationId} does not exists.");
            }

            var group = await groupRepository.GetGroupByChargeStationId(command.ChargeStationId);
            if (group is null)
            {
                throw new ArgumentException($"A Group does not exists.");
            }
            
            var chargeStation = group.ChargeStations.First(cs => cs.Id == command.ChargeStationId);
            
            var connector = ConnectorEntity.Create(connectorName, command.CapacityInAmps);
            chargeStation.AddConnector(connector);
            
            group.UpdateCapacity();

            await connectorRepository.AddConnector(connector);
            
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();

            return Result<ConnectorEntity>.Success(connector);
        }
        catch (ArgumentException ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}