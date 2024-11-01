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

public class DeleteConnectorHandler : IRequestHandler<DeleteConnectorCommand, Result<ConnectorEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    public DeleteConnectorHandler(
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
    
    public async Task<Result<ConnectorEntity>> Handle(DeleteConnectorCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var connector = await _connectorRepository.GetConnectorById(command.Id);
            if (connector is null)
            {
                throw new ArgumentException($"Connector with Id {command.Id} does not exists.");
            }
            
            if (connector.ChargeStationId != command.ChargeStationId)
            {
                throw new ArgumentException($"Connector with Id {command.Id} does not belong to ChargeStation.");
            }
            
            var chargeStation = await _chargeStationRepository.GetChargeStationById(connector.ChargeStationId);
            if (chargeStation is null)
            {
                throw new ArgumentException($"ChargeStation with Id {connector.ChargeStationId} does not exists.");
            }
            
            var group = await _groupRepository.GetGroupById(chargeStation.GroupId);
            if (group is null)
            {
                throw new ArgumentException($"Group with Id {chargeStation.Id} does not exists.");
            }
            
            connector.UpdateStateDelete(RowState.PendingDelete);
            
            var currentChargeStation = group.ChargeStations.First(cs => cs.Id == chargeStation.Id);
            currentChargeStation.RemoveConnector(connector);
            group.UpdateCapacity();
        
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