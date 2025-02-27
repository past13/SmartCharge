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
    
    public static GroupEntity Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        var group = new GroupEntity
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        return group;
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        Name = name;
    }
    
    public void UpdateRowState(RowState rowState)
    {
        RowState = rowState;
        foreach (var chargeStation in _chargeStations)
        {
            chargeStation.UpdateRowState(rowState);
        }
    }
    
    public void IsValidForChange()
    {
        if (RowState == RowState.PendingDelete)
        {
            throw new ArgumentException($"A Group with Id {Id} already deleting.");
        }
    }
    
    public void AddChargeStation(ChargeStationEntity chargeStationEntity)
    {
        if (_chargeStations.Contains(chargeStationEntity))
        {
            throw new ArgumentException($"A ChargeStation already exists.");
        }
        
        _chargeStations.Add(chargeStationEntity);
    }
    
    public void RemoveChargeStation(ChargeStationEntity chargeStationEntity)
    {
        _chargeStations.Remove(chargeStationEntity);
    }
    
    public void UpdateCapacity()
    {
        CapacityInAmps = _chargeStations.Sum(cs => cs.GetTotalCurrentLoad());
    }
}