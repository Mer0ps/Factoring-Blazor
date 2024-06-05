using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Domain.Entities;
public class JobExecution : BaseAuditableEntity
{
    [Key]
    public string Name { get; set; }
    public long TimeSpan { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}
