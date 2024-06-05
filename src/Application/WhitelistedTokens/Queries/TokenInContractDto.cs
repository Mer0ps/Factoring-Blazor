namespace Application.WhitelistedTokens.Queries;
public class TokenInContractDto
{
    public string TokenIdentifer { get; set; }
    public string HTokenIdentifer { get; set; }
    public string Amount { get; set; }
    public string? AmountCollateral { get; set; }
    public string? AmountSupply { get; set; }
    public string? MoneyMarket { get; set; }

}
