using Application.ScAmins.Queries;
using Mx.NET.SDK.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace Mx.Blazor.DApp.Client.Models;

public class FundsModel
{
    [Required(ErrorMessage = "You must set an amount to send")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "Token Identifier is required")]
    public string? TokenIdentifier { get; set; }
    public string? CurrentAmountInSc { get; set; }

    public ESDT? TokenESDT { get; set; }
    public bool IsAdd { get; set; }

    public IEnumerable<WhitelistedTokenDto> TokenIdentifiers { get; set; }
}
