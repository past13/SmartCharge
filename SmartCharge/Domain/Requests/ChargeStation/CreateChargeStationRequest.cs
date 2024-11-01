using System.Collections.Generic;
using SmartCharge.Domain.Requests.Connector;

namespace SmartCharge.Domain.Requests.ChargeStation;

public class CreateChargeStationRequest
{
    public string Name { get; set; }
    public List<ConnectorRequest> Connectors { get; set; } 
}