using Application.Contracts.Queries;
using Application.ScAmins.Queries;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Mx.Blazor.DApp.Client.Models;

public class InvoiceModel
{
    public InvoiceModel()
    {
        Contracts = Enumerable.Empty<ContractDto>();
    }

    public IBrowserFile File { get; set; }
    public decimal Amount { get; set; }
    public long? IdContract { get; set; }
    public string TokenIdentifier { get; set; }

    [Required]
    public DateTime? IssueDate { get; set; }
    [Required]
    public DateTime? DueDate { get; set; }
    public IEnumerable<ContractDto> Contracts { get; set; }
    public IEnumerable<WhitelistedTokenDto> TokenIdentifiers { get; set; }
}
