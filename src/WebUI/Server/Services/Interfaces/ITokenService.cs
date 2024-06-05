using Application.WhitelistedTokens.Queries;

namespace Mx.Blazor.DApp.Services.Interfaces;

public interface ITokenService
{
    Task<IEnumerable<TokenInContractDto>> GetTokenInContract();
}
