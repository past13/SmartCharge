using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Connector;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;
using SmartCharge.UnitOfWork;

namespace SmartCharge.Handlers.Connector;

public class DeleteConnectorHandler : IRequestHandler<DeleteConnectorCommand, Result<ConnectorEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    public DeleteConnectorHandler(
        IUnitOfWork unitOfWork,
        IChargeStationRepository chargeStationRepository,
        IConnectorRepository connectorRepository)
    {
        _unitOfWork = unitOfWork;
        _chargeStationRepository = chargeStationRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<Result<ConnectorEntity>> Handle(DeleteConnectorCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connector = await _connectorRepository.GetConnectorById(command.Id);
            if (connector == null)
            {
                throw new ArgumentException($"Connector with Id {command.Id} does not exists.");
            }
            
            if (connector.ChargeStationId != command.ChargeStationId)
            {
                throw new ArgumentException($"Connector with Id {command.Id} does not belong to ChargeStation.");
            }
            
            
            
            //Todo: set status deleting
        
            await _connectorRepository.DeleteConnectorById(command.Id);
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result<ConnectorEntity>.Success(null);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ConnectorEntity>.Failure(ex.Message);
        }
    }
}