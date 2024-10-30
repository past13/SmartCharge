using System;

namespace SmartCharge.DTOs;

public class ConnectorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    
    public Guid ChargeStationId { get; set; }
}