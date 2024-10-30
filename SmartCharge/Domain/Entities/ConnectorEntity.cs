using System;

namespace SmartCharge.Domain.Entities;

public class ConnectorEntity : BaseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public int MaxCurrentInAmps { get; private set; }
    
    public Guid ChargeStationId { get; set; }
    public ChargeStationEntity ChargeStation { get; set; }
    
    public static ConnectorEntity Create(string name, int amps)
    {
        var connector = new ConnectorEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
        };

        return connector;
    }
    
    public void UpdateMaxCurrent(int newMaxCurrentInAmps)
    {
        if (newMaxCurrentInAmps <= 0)
            throw new ArgumentOutOfRangeException(nameof(newMaxCurrentInAmps), "Max current must be greater than zero.");

        MaxCurrentInAmps = newMaxCurrentInAmps;
    }
}