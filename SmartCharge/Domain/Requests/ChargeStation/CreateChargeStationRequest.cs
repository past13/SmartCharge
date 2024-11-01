using System;
using System.Collections.Generic;

namespace SmartCharge.Domain.Requests.ChargeStation;

public class CreateChargeStationRequest
{
    public string Name { get; set; }
    public Guid GroupId { get; set; }
    public List<ConnectorRequest> Connectors = []; 
}