using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;

namespace SmartCharge.Commands.Group;

public class UpdateGroupCommand : IRequest<GroupEntity>
{
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    public List<ChargeStationRequest> ChargeStations { get; set; } 
    
    public UpdateGroupCommand(string name, int capacityInAmps, List<ChargeStationRequest> chargeStations)
    {
        Name = name;
        CapacityInAmps = capacityInAmps;
        ChargeStations = chargeStations;
    }
}
