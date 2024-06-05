using System.ComponentModel.DataAnnotations;

namespace Mx.Blazor.DApp.Client.Models;

public class AdministratorModel
{
    [Required(ErrorMessage = "You must provide an address")]
    public string? Address { get; set; }
}
