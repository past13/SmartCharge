using System;
using System.Collections.Generic;

namespace SmartCharge.DTOs;

public class ChargeStationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ConnectorDTO> Connectors { get; set; } = new List<ConnectorDTO>();
}