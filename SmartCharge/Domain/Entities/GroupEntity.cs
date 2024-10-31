using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SmartCharge.Domain.Entities;

public class GroupEntity : BaseEntity
{
    private readonly List<ChargeStationEntity> _chargeStations = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int CapacityInAmps { get; private set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
    
    public IReadOnlyCollection<ChargeStationEntity> ChargeStations => _chargeStations.ToList(); //Todo AsReadonly?
    
    public static GroupEntity Create(string name, int capacityInAmps)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        var group = new GroupEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            CapacityInAmps = capacityInAmps
        };

        return group;
    }

    public void Update(string name, int capacityInAmps)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        Name = name;
        CapacityInAmps = capacityInAmps;
    }
    
    public void AddChargeStation(ChargeStationEntity chargeStationEntity)
    {
        _chargeStations.Add(chargeStationEntity);
    }
    
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