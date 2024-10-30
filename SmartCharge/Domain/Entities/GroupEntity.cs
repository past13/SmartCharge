using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCharge.Domain.Entities;

public class GroupEntity : BaseEntity
{
    private readonly List<ChargeStationEntity> _chargeStations = [];

    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; private set; }
    public IReadOnlyCollection<ChargeStationEntity> ChargeStations => _chargeStations.ToList(); //Todo AsReadonly?
    
    public static GroupEntity Create(string name, int capacityInAmps)
    {
        var group = new GroupEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            CapacityInAmps = capacityInAmps
        };

        return group;
    }
    
    public void AddChargeStation(ChargeStationEntity chargeStationEntity)
    {
        //Todo: Can add chargeStation with connectors
        _chargeStations.Add(chargeStationEntity);
    }
    
    public void UpdateName(string newName) => Name = newName;
    
    // public void UpdateCapacity(int newCapacity)
    // {
    //     if (newCapacity < _chargeStations.Sum(cs => cs.GetTotalCurrentLoad()))
    //     {
    //         throw new InvalidOperationException("New capacity is too low.");
    //     }
    //     CapacityInAmps = newCapacity;
    // }
    
    // public void AddChargeStation(ChargeStation chargeStation)
    // {
    //     if (_chargeStations.Any(cs => cs.Id == chargeStation.Id))
    //         throw new InvalidOperationException("Charge station already exists in this group.");
    //
    //     if (_chargeStations.Sum(cs => cs.TotalConnectorLoad) + chargeStation.TotalConnectorLoad > CapacityInAmps)
    //         throw new InvalidOperationException("Adding this charge station would exceed the group's capacity.");
    //
    //     _chargeStations.Add(chargeStation);
    // }
    
    // public void RemoveChargeStation(Guid chargeStationId)
    // {
    //     var chargeStation = _chargeStations.FirstOrDefault(cs => cs.Id == chargeStationId);
    //     if (chargeStation == null)
    //         throw new InvalidOperationException("Charge station not found in this group.");
    //
    //     _chargeStations.Remove(chargeStation);
    // }
}