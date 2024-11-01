using SmartCharge.Domain.Requests.ChargeStation;

namespace SmartCharge.Domain.Requests.Group;

public class CreateGroupRequest
{
    public string Name { get; set; }
    public GetChargeStationRequest ChargeStation { get; set; } 
}