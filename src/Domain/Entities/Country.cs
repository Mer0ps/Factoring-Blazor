using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Country : BaseAuditableEntity
{
    [Key]
    public string Code { get; set; }
    public string Name { get; set; }
    public ICollection<LegalForm> LegalForms { get; set; }
}
