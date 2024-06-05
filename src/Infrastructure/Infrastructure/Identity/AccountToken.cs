namespace Infrastructure.Identity;

public class AccountToken
{
    public string Address { get; set; } = "";
    public string? Signature { get; set; }

    public bool IsValid()
        => Address != "" && !string.IsNullOrEmpty(Signature);
}
