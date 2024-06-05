using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Domain.Entities;
public class CollectedFee : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }
    public long InvoiceId { get; set; }
    public long ContractId { get; set; }

    public BigInteger CommissionFee { get; set; }

    public BigInteger FinancingFee { get; set; }

    public virtual Invoice Invoice { get; set; }

}
