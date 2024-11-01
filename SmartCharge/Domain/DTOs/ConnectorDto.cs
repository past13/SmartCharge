using System;

namespace SmartCharge.Domain.DTOs;

public class ConnectorDto
{
    public Guid Id { get; set; }
    public Guid ChargeStationId { get; set; }
    public string Name { get; set; }
    public int MaxCurrentInAmps { get; set; }
    public int ConnectorNumber { get; set; }
}