using System;

namespace SmartCharge.Domain.Requests;

public class ConnectorRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public int MaxCapacityInAmps { get; set; }
}