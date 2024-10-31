using System;

namespace SmartCharge.Domain.Entities;

public class ConnectorEntity : BaseEntity
{
    public Guid Id { get; set; }
    public int ConnectorNumber { get; private set; } 
    public string Name { get; set; }
    public int MaxCapacityInAmps { get; private set; }
    public Guid ChargeStationId { get; set; }
    public ChargeStationEntity ChargeStation { get; set; }
    
    public static ConnectorEntity Create(string name, int capacityInAmps)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        var connector = new ConnectorEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            MaxCapacityInAmps = capacityInAmps
        };

        return connector;
    }
    
    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        Name = name;
    }

    public void UpdateConnectorNumber(int connectorNumber)
    {
        ConnectorNumber = connectorNumber;
    }
    
    public void UpdateMaxCurrent(int newMaxCurrentInAmps)
    {
        if (newMaxCurrentInAmps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newMaxCurrentInAmps), "Max current must be greater than zero.");
        }

        MaxCapacityInAmps = newMaxCurrentInAmps;
    }
}