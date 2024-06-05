using System.ComponentModel.DataAnnotations;

namespace Mx.Blazor.DApp.Client.Models;

public class HatomAdminModel
{
    [Required(ErrorMessage = "You must provide an address")]
    public string? Address { get; set; }
    public string? CurrentAddress { get; set; }
}
