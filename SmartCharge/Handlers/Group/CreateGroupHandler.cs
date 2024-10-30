using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class CreateGroupHandler : IRequestHandler<CreateGroupCommand, GroupEntity>
{
    private readonly IGroupRepository _groupRepository;
    public CreateGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<GroupEntity> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        //Todo: validate if group not exist same name
        var group = GroupEntity.Create(command.Name, command.CapacityInAmps);

        //Todo: validate is just one chargeStation
        //Todo: validate if chargeStation not exist same name
        
        var chargeStationEntity = ChargeStationEntity.Create(command.ChargeStation.Name);
        
        group.AddChargeStation(chargeStationEntity);
        
        await _groupRepository.AddGroup(group);
        
        return new GroupEntity();
    }
}