using Domain.Common;

namespace Domain.Entities;
public class InvoiceHistory : BaseAuditableEntity
{
    public long Id { get; set; }
    public string TxHash { get; set; }
    public Status? Status { get; set; }
    public long InvoiceId { get; set; }
    public long ContractId { get; set; }
    public DateTime TxExecuteAt { get; set; }

    public virtual Invoice Invoice { get; set; }
}
