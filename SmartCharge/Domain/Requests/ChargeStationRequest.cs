using System;
using System.Collections.Generic;

namespace SmartCharge.Domain.Requests;

public class ChargeStationRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ConnectorRequest> Connectors { get; set; } 
}