using SmartCharge.Domain.Requests.ChargeStation;

namespace SmartCharge.Domain.Requests.Group;

public class CreateGroupRequest
{
    public string Name { get; set; }
    public ChargeStationRequest ChargeStation { get; set; } 
}