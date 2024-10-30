using System.ComponentModel.DataAnnotations;

namespace SmartCharge.Domain.Entities;

public class BaseEntity
{
    [Timestamp]
    public byte[] RowVersion { get; set; } 
}