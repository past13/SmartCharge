using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartCharge.Requests;

public class GroupRequest
{
    public string Name { get; set; }
    
    public List<ChargeStationRequest> ChargeStations { get; set; } 
}