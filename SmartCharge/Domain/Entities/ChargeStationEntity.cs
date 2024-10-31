using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCharge.Domain.Entities;

public class ChargeStationEntity : BaseEntity
{
    private readonly List<ConnectorEntity> _connectors = [];

    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid GroupId { get; set; } 
    public GroupEntity GroupEntity { get; set; }
    
    public IReadOnlyCollection<ConnectorEntity> Connectors => _connectors.ToList(); //Todo AsReadonly?

    public static ChargeStationEntity Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        var chargeStation = new ChargeStationEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
        };

        return chargeStation;
    }
    
    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }
        
        Name = name;
    }
    
    public void AddConnector(ConnectorEntity connector)
    {
        if (_connectors.Count > 5)
        {
            throw new InvalidOperationException("A charge station cannot have more than 5 connectors.");
        }

        if (_connectors.Any(c => c.Id == connector.Id))
        {
            throw new InvalidOperationException($"A connector with ID {connector.Id} already exists in this charge station.");
        }

        var nextConnectorNumber = _connectors.Count + 1;
        connector.UpdateConnectorNumber(nextConnectorNumber);
        
        _connectors.Add(connector);
    }
}