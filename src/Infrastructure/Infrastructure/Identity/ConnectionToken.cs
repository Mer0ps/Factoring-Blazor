namespace Infrastructure.Identity;

public class ConnectionToken
{
    public AccountToken AccountToken { get; set; }
    public string AccessToken { get; set; }

    public ConnectionToken(AccountToken accountToken,
                           string accessToken)
    {
        AccountToken = accountToken;
        AccessToken = accessToken;
    }
}
