using System;

namespace SmartCharge.Domain.Entities;

public class ConnectorEntity : BaseEntity
{
    public Guid Id { get; private set; }
    public int ConnectorNumber { get; private set; } 
    public string Name { get; private set; }
    public int MaxCurrentInAmps { get; private set; }
    public Guid ChargeStationId { get; set; }
    public ChargeStationEntity ChargeStation { get; set; }
    
    public static ConnectorEntity Create(string name, int maxCurrentInAmps)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        ValidateMaxCurrentInAmps(maxCurrentInAmps);
        
        var connector = new ConnectorEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            MaxCurrentInAmps = maxCurrentInAmps
        };

        return connector;
    }
    
    public void Update(string name, int maxCurrentInAmps)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        ValidateMaxCurrentInAmps(maxCurrentInAmps);
        
        Name = name;
        MaxCurrentInAmps = maxCurrentInAmps;
    }
    
    public void UpdateStateDelete(RowState rowState)
    {
        RowState = rowState;
    }

    public void UpdateConnectorNumber(int connectorNumber)
    {
        ConnectorNumber = connectorNumber;
    }

    private static void ValidateMaxCurrentInAmps(int newMaxCurrentInAmps)
    {
        if (newMaxCurrentInAmps <= 0)
        {
            throw new ArgumentException("Max current must be greater than zero.", nameof(newMaxCurrentInAmps));
        }
    }
}