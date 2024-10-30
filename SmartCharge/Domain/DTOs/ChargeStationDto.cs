using System;
using System.Collections.Generic;

namespace SmartCharge.Domain.DTOs;

public class ChargeStationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ConnectorDto> Connectors { get; set; } = new List<ConnectorDto>();
}