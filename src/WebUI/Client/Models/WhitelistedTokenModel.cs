using System.ComponentModel.DataAnnotations;

namespace Mx.Blazor.DApp.Client.Models;

public class WhitelistedTokenModel
{
    [Required(ErrorMessage = "You must provide a token identifier")]
    public string? TokenIdentifier { get; set; }
}
