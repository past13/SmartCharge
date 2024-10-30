using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.ChargeStation;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.ChargeStation;

public class CreateChargeStationHandler : IRequestHandler<CreateChargeStationCommand, ApiResponse<ChargeStationEntity>>
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
    
    public async Task<ApiResponse<ChargeStationEntity>> Handle(CreateChargeStationCommand command, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<ChargeStationEntity>();

        var group = await _groupRepository.GetGroupById(command.GroupId);
        if (group == null)
        {
            response.Error = $"A Group does not exists.";
            return response;
        }

        var chargeStationName = command.Name.Trim();
        var chargeStationNameExist = await _chargeStationRepository.IsNameExist(chargeStationName);
        if (chargeStationNameExist)
        {
            response.Error = $"A ChargeStation with the name '{chargeStationName}' already exists.";
            return response; 
        }
        
        var chargeStation = ChargeStationEntity.Create(chargeStationName);
        group.AddChargeStation(chargeStation);
        
        await _groupRepository.UpdateGroup(group);
        
        response.Data = chargeStation;
        
        return response;
    }
}