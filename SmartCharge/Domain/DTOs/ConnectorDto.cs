using System;

namespace SmartCharge.Domain.DTOs;

public class ConnectorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
    
    public Guid ChargeStationId { get; set; }
}