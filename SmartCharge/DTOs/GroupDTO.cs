using System;
using System.Collections.Generic;
using SmartCharge.DTOs;

namespace SmartCharge.Repository;

public class GroupDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; private set; }
    public List<ChargeStationDTO> ChargeStations { get; set; } = new List<ChargeStationDTO>();
}