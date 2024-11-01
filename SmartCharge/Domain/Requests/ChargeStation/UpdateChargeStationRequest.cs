using System;

namespace SmartCharge.Domain.Requests.ChargeStation;

public class UpdateChargeStationRequest
{
    public Guid Id { get; set; }
    public Guid GroupId  { get; set; }
    public string Name { get; set; }
}