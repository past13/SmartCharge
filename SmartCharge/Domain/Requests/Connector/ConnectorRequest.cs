namespace SmartCharge.Domain.Requests.Connector;

public class ConnectorRequest
{
    public string Name { get; set; } 
    public int MaxCapacityInAmps { get; set; }
}