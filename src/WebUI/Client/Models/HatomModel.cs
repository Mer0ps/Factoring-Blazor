using Application.WhitelistedTokens.Queries;

namespace Mx.Blazor.DApp.Client.Models;

public class HatomModel
{
    public IEnumerable<TokenInContractDto> TokenInContracts { get; set; }
}
