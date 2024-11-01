namespace SmartCharge.Domain.Requests.Connector;

public class UpdateConnectorRequest
{
    public string Name { get; set; }
    public int MaxCurrentInAmps { get; set; }
}