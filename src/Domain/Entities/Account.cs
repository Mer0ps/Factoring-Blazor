using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Account : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long? OnChainId { get; set; }

    ///////////////////////////////////////////////////////////////////////////////////////////////

    // Company name
    public string CompanyName { get; set; }

    // VAT number for intra-EU transactions
    public string VATNumber { get; set; }

    // Registration number (varies by country)
    public string RegistrationNumber { get; set; }
    public int LegalFormId { get; set; }
    public int BusinessActivityId { get; set; }
    public virtual LegalForm LegalForm { get; set; }
    public virtual BusinessActivity BusinessActivity { get; set; }


    ///////////////////////////////////////////////////////////////////////////////////////////////


    public bool IsKyc { get; set; }
    public string WithdrawAddress { get; set; }
    public int? Fee { get; set; }
    public int? Score { get; set; }

    /// <summary>
    /// List of address that can manage the account
    /// </summary>
    public ICollection<Administrator> Administrators { get; set; }

    public ICollection<Contract> SupplierContracts { get; }
    public ICollection<Contract> ClientContracts { get; }
}
