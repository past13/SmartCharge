using System.Collections.Generic;

namespace SmartCharge.Domain.Requests;

public class GroupRequest
{
    public string Name { get; set; }
    
    public List<ChargeStationRequest> ChargeStations { get; set; } 
}