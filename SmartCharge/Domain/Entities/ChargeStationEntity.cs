using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCharge.Domain.Entities;

public class ChargeStationEntity : BaseEntity
{
    private readonly List<ConnectorEntity> _connectors = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid GroupId { get; set; } 
    public GroupEntity GroupEntity { get; private set; }
    
    public IReadOnlyCollection<ConnectorEntity> Connectors => _connectors.ToList(); 

    public static ChargeStationEntity Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        var chargeStation = new ChargeStationEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
        };

        return chargeStation;
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }
        
        Name = name;
    }
    
    public void AddConnector(ConnectorEntity connector)
    {
        if (_connectors.Count >= 5)
        {
            throw new ArgumentException("A charge station cannot have more than 5 connectors.");
        }

        if (_connectors.Any(c => c.Id == connector.Id))
        {
            throw new ArgumentException($"A connector with ID {connector.Id} already exists in this charge station.");
        }

        var nextConnectorNumber = _connectors.Count + 1;
        connector.UpdateConnectorNumber(nextConnectorNumber);
        
        _connectors.Add(connector);
    }

    public void RemoveConnector(ConnectorEntity connector)
    {
        if (_connectors.Count <= 1)
        {
            throw new ArgumentException("A charge station cannot have more than 5 connectors.");
        }
        
        _connectors.Remove(connector);
    }

    public int GetTotalCurrentLoad()
    {
        return _connectors.Sum(c => c.MaxCurrentInAmps);
    }

    public void UpdateGroup(Guid groupId)
    {
        GroupId = groupId;
    }
}