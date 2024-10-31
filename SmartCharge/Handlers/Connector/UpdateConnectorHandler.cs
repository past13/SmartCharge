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
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;

    public UpdateConnectorHandler(
        IUnitOfWork unitOfWork,
        IConnectorRepository connectorRepository)
    {
        _unitOfWork = unitOfWork;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(UpdateConnectorCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connectorName = command.Name.Trim();
            var connectorNameExist = await _connectorRepository.IsNameExist(connectorName);
            if (connectorNameExist)
            {
                throw new ArgumentException($"A Connector with the name {connectorName} already exists.");
            }
        
            var connector = await _connectorRepository.GetConnectorById(command.Id);
            if (connector is null)
            {
                throw new ArgumentException($"A Connector with Id {command.Id} does not exists.");
            }

            var newChargeStation = await _chargeStationRepository.IsChargeStationExist(command.ChargeStationId);
            if (!newChargeStation)
            {
                throw new ArgumentException($"A ChargeStation with Id {command.ChargeStationId} does not exists.");
            }
            
            var currentChargeStation = await _chargeStationRepository.GetChargeStationById(connector.ChargeStationId);
            if (currentChargeStation?.Connectors.Count is 1)
            {
                throw new ArgumentException($"Connector can not be deleted ChargeStation required at least one connector.");
            }
            
            
            
            
            
            currentChargeStation?.RemoveConnector(connector);
            
            connector.Update(connectorName, command.MaxCurrentInAmps);
            connector.UpdatechargeStation(command.ChargeStationId);
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            
            return Result<ConnectorEntity>.Success(null);
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