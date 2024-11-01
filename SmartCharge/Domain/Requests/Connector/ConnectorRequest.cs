using System;

namespace SmartCharge.Domain.Requests.Connector;

public class ConnectorRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public int MaxCapacityInAmps { get; set; }
}