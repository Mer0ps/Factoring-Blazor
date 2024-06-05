using Domain.Common;
using System.Numerics;

namespace Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public long Id { get; set; }
    public string Hash { get; set; }
    public BigInteger Amount { get; set; }
    public string Identifier { get; set; }
    public DateTime DueDate { get; set; }
    public long ContractId { get; set; }
    public int EuriborRate { get; set; }
    public virtual Contract Contract { get; set; }
    public ICollection<InvoiceHistory> InvoiceHistories { get; set; }
    public ICollection<CollectedFee> CollectedFees { get; set; }
}

public enum Status
{
    PendingValidation,
    Valid,
    PartiallyFunded,
    Payed,
    FullyFunded,
    Refused
}
