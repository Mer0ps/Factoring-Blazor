using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Administrator : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }
    public string Address { get; set; }

    public long IdAccount { get; set; }

    public Account Account { get; set; }
}
