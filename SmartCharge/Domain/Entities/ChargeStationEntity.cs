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
        var chargeStation = new ChargeStationEntity
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        return chargeStation;
    }
    
    public void AddConnector(ConnectorEntity connector)
    {
        //Todo: Can add chargeStation with connectors
        _connectors.Add(connector);
    }
}