using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;
using SmartCharge.Requests;

namespace SmartCharge.Commands;

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
