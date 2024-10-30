using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, ApiResponse<GroupEntity>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    private readonly IConnectorRepository _connectorRepository;
    
    public CreateGroupHandler(
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository,
        IConnectorRepository connectorRepository)
    {
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
        _connectorRepository = connectorRepository;
    }
    
    public async Task<ApiResponse<GroupEntity>> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<GroupEntity>();
        var groupName = command.Name.Trim();
        
        var groupNameExist = await _groupRepository.IsNameExist(groupName);
        if (groupNameExist)
        {
            response.Error = $"A Group with the name '{groupName}' already exists.";
            return response; 
        }

        var group = GroupEntity.Create(groupName, command.CapacityInAmps);
        
        if (command.ChargeStation != null)
        {
            var chargeStationName = command.ChargeStation.Name.Trim();
            var chargeStationExist = await _chargeStationRepository.IsNameExist(chargeStationName);
            if (chargeStationExist)
            {
                response.Error = $"A ChargeStation with the name '{chargeStationName}' already exists.";
                return response; 
            }
            
            var chargeStation = ChargeStationEntity.Create(chargeStationName);
            group.AddChargeStation(chargeStation);
        }
        
        await _groupRepository.AddGroup(group);
        
        response.Data = group;
        
        return response;
    }
}