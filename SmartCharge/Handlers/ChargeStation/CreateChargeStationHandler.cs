using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SmartCharge.Commands;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers;

public class CreateChargeStationHandler : IRequestHandler<CreateChargeStationCommand, ChargeStationEntity>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;
    
    public CreateChargeStationHandler(
        IChargeStationRepository chargeStationRepository,
        IGroupRepository groupRepository)
    {
        _chargeStationRepository = chargeStationRepository;
        _groupRepository = groupRepository;
    }
    
    public async Task<ChargeStationEntity> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupById(command.GroupId);
        if (group == null)
        {
            //Todo: if group dont exist cant create station
            return null;
        }
        
        //Todo: validate is just one chargeStation
        //Todo: validate if chargeStation not exist same name
        
        var chargeStation = ChargeStationEntity.Create(command.Name);
        group.AddChargeStation(chargeStation);

        // chargeStation.AddConnector();
        
        await _groupRepository.AddGroup(group);
        return chargeStation;
    }
}