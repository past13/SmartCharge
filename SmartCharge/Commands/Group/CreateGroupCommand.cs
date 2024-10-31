using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class CreateGroupCommand : IRequest<Result<GroupEntity>>
{
    public string Name { get; set; }
    public ChargeStationRequest ChargeStation { get; set; } 
    
    public CreateGroupCommand(string name, ChargeStationRequest chargeStation)
    {
        Name = name;
        ChargeStation = chargeStation;
    }
}
