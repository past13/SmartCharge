using MediatR;
using SmartCharge.Domain.DTOs;
using SmartCharge.Domain.Requests.ChargeStation;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class CreateGroupCommand : IRequest<Result<GroupDto>>
{
    public string Name { get; set; }
    public GetChargeStationRequest ChargeStation { get; set; } 
    
    public CreateGroupCommand(string name, GetChargeStationRequest chargeStation)
    {
        Name = name;
        ChargeStation = chargeStation;
    }
}
