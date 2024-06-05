using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class WhitelistedToken : BaseAuditableEntity
{
    [Key]
    public string TokenIdentifier { get; set; }
    public string? MoneyMarketAddress { get; set; }
    public string? HTokenIdentifier { get; set; }
}
