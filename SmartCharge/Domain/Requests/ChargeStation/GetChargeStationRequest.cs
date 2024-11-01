using System;
using System.Collections.Generic;

namespace SmartCharge.Domain.Requests.ChargeStation;

public class GetChargeStationRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ConnectorRequest> Connectors { get; set; } 
}