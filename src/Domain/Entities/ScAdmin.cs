using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class ScAdmin : BaseAuditableEntity
{
    [Key]
    public string Address { get; set; }
}
