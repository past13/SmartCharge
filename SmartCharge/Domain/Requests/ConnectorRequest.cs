namespace SmartCharge.Domain.Requests;

public class ConnectorRequest
{
    public string Name { get; set; } 
    public int MaxCapacityInAmps { get; set; }
}