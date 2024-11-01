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

public class CreateChargeStationHandler : IRequestHandler<CreateChargeStationCommand, Result<ChargeStationEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public CreateChargeStationHandler(
        IUnitOfWork unitOfWork,
        IChargeStationRepository chargeStationRepository,
        IGroupRepository groupRepository)
    {
        _unitOfWork = unitOfWork;
        _chargeStationRepository = chargeStationRepository;
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<ChargeStationEntity>> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var group = await _groupRepository.GetGroupById(command.GroupId);
            if (group == null)
            {
                throw new ArgumentException($"A Group with Id {command.GroupId} does not exists.");
            }

            var chargeStationName = command.Name.Trim();
            var chargeStationNameExist = await _chargeStationRepository.IsNameExist(chargeStationName);
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
            
            await _chargeStationRepository.AddChargeStation(chargeStation);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            
            return Result<ChargeStationEntity>.Success(chargeStation);
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return Result<ChargeStationEntity>.Failure(ex.Message);
        }
    }
}