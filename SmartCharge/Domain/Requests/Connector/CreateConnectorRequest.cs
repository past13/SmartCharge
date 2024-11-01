using System;

namespace SmartCharge.Domain.Requests.Connector;

public class CreateConnectorRequest
{
    public string Name { get; set; }
    public int CapacityInAmps { get; set; }
}