using System;

namespace SmartCharge.Domain.Requests.Group;

public class UpdateGroupRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}