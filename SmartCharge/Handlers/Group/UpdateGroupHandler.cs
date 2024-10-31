using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, Result<GroupEntity>>
{
    private readonly IGroupRepository _groupRepository;
    public UpdateGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<GroupEntity>> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var response = new Result<GroupEntity>();

        var groupName = command.Name.Trim();
        var groupNameExist = await _groupRepository.IsNameExist(groupName);
        if (groupNameExist)
        {
            response.Error = $"A Group with the name '{groupName}' already exists.";
            return response; 
        }
        
        var group = await _groupRepository.GetGroupById(command.Id);
        if (group == null)
        {
            response.Error = $"A Group does not exists.";
            return response; 
        }

        group.Update(groupName);

        // foreach (var chargeStationRequest in command.ChargeStations)
        // {
        //     var chargeStation = ChargeStationEntity.Create(chargeStationRequest.Name);
        //     group.AddChargeStation(chargeStation);
        // }
        
        var result = await _groupRepository.UpdateGroup(group);
        return result;
    }
}