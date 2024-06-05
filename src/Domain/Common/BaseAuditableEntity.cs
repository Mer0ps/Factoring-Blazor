namespace Domain.Common;

public abstract class BaseAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}

