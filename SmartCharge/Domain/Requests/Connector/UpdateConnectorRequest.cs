using System;

namespace SmartCharge.Domain.Requests.Connector;

public class UpdateConnectorRequest
{
    public Guid Id { get; set; }
    public Guid ChargeStationId  { get; set; }
    public string Name { get; set; }
    public int MaxCurrentInAmps { get; set; }
}