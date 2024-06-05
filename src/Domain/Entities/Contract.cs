using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class Contract : BaseAuditableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public long Id { get; set; }
    public long AccountSupplierId { get; set; }
    public long AccountClientId { get; set; }
    public bool IsSigned { get; set; }

    public Account AccountSupplier { get; set; }

    public Account AccountClient { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
}
