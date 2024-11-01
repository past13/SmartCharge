using System.ComponentModel.DataAnnotations;

namespace SmartCharge.Domain.Entities;

public enum RowState 
{
    PendingDelete,
    Active
}

public class BaseEntity
{
    [Timestamp]
    public byte[] RowVersion { get; set; } 
    public RowState RowState { get; set; } = RowState.Active;
}