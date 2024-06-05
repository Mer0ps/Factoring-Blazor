using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class BusinessActivity : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public ICollection<Account> Accounts { get; set; }
}
