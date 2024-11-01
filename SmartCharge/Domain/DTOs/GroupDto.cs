using System;
using System.Collections.Generic;

namespace SmartCharge.Domain.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CapacityInAmps { get; private set; }
    public List<ChargeStationDto> ChargeStations { get; set; } = [];
}