using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;

namespace SmartCharge.Commands.Group;

public class CreateGroupCommand : IRequest<GroupEntity>
{
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    public ChargeStationRequest ChargeStation { get; set; } 
    
    public CreateGroupCommand(string name, int capacityInAmps, ChargeStationRequest chargeStation)
    {
        Name = name;
        CapacityInAmps = capacityInAmps;
        ChargeStation = chargeStation;
    }
}
