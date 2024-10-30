using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;
using SmartCharge.Requests;

namespace SmartCharge.Commands;

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
