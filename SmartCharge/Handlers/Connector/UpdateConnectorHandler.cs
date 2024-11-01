using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class UpdateConnectorHandler : IRequestHandler<UpdateConnectorCommand, Result<ConnectorEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public UpdateConnectorHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository,
        IConnectorRepository connectorRepository)
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(UpdateConnectorCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connectorName = command.Name.Trim();
            var connectorNameExist = await _connectorRepository.IsNameExist(connectorName, command.Id);
            if (connectorNameExist)
            {
                throw new ArgumentException($"A Connector with the name {connectorName} already exists.");
            }
        
            var connector = await _connectorRepository.GetConnectorById(command.Id);
            if (connector is null)
            {
                throw new ArgumentException($"A Connector with Id {command.Id} does not exists.");
            }
            
            var newChargeStation = await _chargeStationRepository.GetChargeStationById(command.ChargeStationId);
            if (newChargeStation is null)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.ChargeStationId} does not exists.");
            }
            
            var group = await _groupRepository.GetGroupByChargeStationId(connector.ChargeStationId);
            if (group is null)
            {
                throw new ArgumentException($"A Group with ChargeStationId {connector.ChargeStationId} does not exists.");
            }
            
            if (connector.ChargeStationId != command.ChargeStationId)
            {
                connector.ChargeStation.RemoveConnector(connector);
                newChargeStation.AddConnector(connector);
                
                group.UpdateCapacity();
                newChargeStation.GroupEntity.UpdateCapacity();
            }
            
            connector.IsValidForChange();
            
            connector.Update(connectorName, command.MaxCurrentInAmps);
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            
            return Result<ConnectorEntity>.Success(connector);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure("The Connector was modified by another user since you loaded it. Please reload the data and try again.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}