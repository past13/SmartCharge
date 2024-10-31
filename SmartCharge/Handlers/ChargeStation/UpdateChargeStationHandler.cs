using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.ChargeStation;

public class UpdateChargeStationHandler : IRequestHandler<UpdateChargeStationCommand, Result<ChargeStationEntity>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    public UpdateChargeStationHandler(
        IGroupRepository groupRepository,
        IChargeStationRepository chargeStationRepository
        )
    {
        _groupRepository = groupRepository;
        _chargeStationRepository = chargeStationRepository;
    }
    
    public async Task<Result<ChargeStationEntity>> Handle(UpdateChargeStationCommand command, CancellationToken cancellationToken)
    {
        var response = new Result<ChargeStationEntity>();

        var chargeStationName = command.Name.Trim();
        var chargeStationExist = await _chargeStationRepository.IsNameExist(chargeStationName);
        if (chargeStationExist)
        {
            response.Error = $"A ChargeStation with the name '{chargeStationName}' already exists.";
            return response; 
        }
        
        var chargeStation = await _chargeStationRepository.GetChargeStationById(command.Id);
        if (chargeStation == null)
        {
            response.Error = $"A ChargeStation does not exists.";
            return response; 
        }
        
        chargeStation.Update(chargeStationName);

        // foreach (var connectorRequest in command.Connectors)
        // {
        //     var connectorEntity = ConnectorEntity.Create(connectorRequest.Name, connectorRequest.CapacityInAmps);
        //     chargeStation.AddConnector(connectorEntity);
        // }
        
        //Todo: update many
        var result = await _chargeStationRepository.UpdateChargeStation(chargeStation);
        return result;
    }
}