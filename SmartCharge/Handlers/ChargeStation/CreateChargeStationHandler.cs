using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.ChargeStation;

public class CreateChargeStationHandler : IRequestHandler<CreateChargeStationCommand, Result<ChargeStationEntity>>
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
    
    public async Task<Result<ChargeStationEntity>> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        var response = new Result<ChargeStationEntity>();

        var group = await _groupRepository.GetGroupById(command.GroupId);
        if (group == null)
        {
            response.Error = $"A Group with Id {command.GroupId} does not exists.";
            return response;
        }

        var chargeStationName = command.Name.Trim();
        var chargeStationNameExist = await _chargeStationRepository.IsNameExist(chargeStationName);
        if (chargeStationNameExist)
        {
            response.Error = $"A ChargeStation with the name {chargeStationName} already exists.";
            return response; 
        }
        
        var chargeStation = ChargeStationEntity.Create(chargeStationName);

        foreach (var connectorRequest in command.Connectors)
        {
            var connector = ConnectorEntity.Create(connectorRequest.Name, connectorRequest.MaxCapacityInAmps);
            chargeStation.AddConnector(connector);
        }
        
        await _chargeStationRepository.AddChargeStation(command.GroupId, chargeStation);
        
        response.Data = chargeStation;
        
        return response;
    }
}