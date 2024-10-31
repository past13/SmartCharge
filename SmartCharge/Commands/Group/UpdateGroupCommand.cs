using System;
using System.Collections.Generic;
using MediatR;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Requests;
using SmartCharge.Domain.Response;

namespace SmartCharge.Commands.Group;

public class UpdateGroupCommand : IRequest<Result<GroupEntity>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    public List<ChargeStationRequest> ChargeStations { get; set; } 
    
    public UpdateGroupCommand(Guid id, string name, int capacityInAmps, List<ChargeStationRequest> chargeStations)
    {
        Id = id;
        Name = name;
        CapacityInAmps = capacityInAmps;
        ChargeStations = chargeStations;
    }
}
