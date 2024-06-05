using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class LegalForm : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public virtual Country Country { get; set; }
    public ICollection<Account> Accounts { get; set; }
}
