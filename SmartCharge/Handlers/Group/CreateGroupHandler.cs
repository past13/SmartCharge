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

public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, Result<GroupEntity>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    public CreateGroupHandler(
        IUnitOfWork unitOfWork,
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository)
    {
        _unitOfWork = unitOfWork;
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<GroupEntity>> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        var response = new Result<GroupEntity>();
        
        try
        {
            var groupName = command.Name.Trim();

            var groupNameExist = await _groupRepository.IsNameExist(groupName);
            if (groupNameExist)
            {
                throw new ArgumentException($"A Group with the name '{groupName}' already exists.");
            }

            var group = GroupEntity.Create(groupName);

            if (command.ChargeStation is not null)
            {
                var chargeStationName = command.ChargeStation.Name.Trim();
                var chargeStationExist = await _chargeStationRepository.IsNameExist(chargeStationName);
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

            await _groupRepository.AddGroup(group);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            response.Data = group;

            return response;
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackAsync();
            // return Result<Group>.Failure("An error occurred while creating the group. Please try again later.");

            response.Error = ex.Message;
            return response;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            
            response.Error = ex.Message;
            return response;
        }
    }
}