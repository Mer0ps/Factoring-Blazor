using Application.ScAmins.Queries;
using Application.WhitelistedTokens.Queries;
using Domain.Constants;
using MediatR;
using Mx.Blazor.DApp.Properties;
using Mx.Blazor.DApp.Services.Interfaces;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.SmartContracts;
using Mx.NET.SDK.Provider;
using System.Globalization;
using System.Text;

namespace Mx.Blazor.DApp.Server.Services;

public class TokenService : ITokenService
{
    private readonly ApiProvider _apiProvider;
    private readonly IMediator _mediator;

    public TokenService(IMediator mediator)
    {
        _apiProvider = new(new ApiNetworkConfiguration(ContractConstants.NETWORK));
        _mediator = mediator;
    }

    public async Task<IEnumerable<TokenInContractDto>> GetTokenInContract()
    {
        var tokenInContractDto = new List<TokenInContractDto>();
        var tokens = await _apiProvider.GetAccountTokens(ContractConstants.FACTORING_CONTRACT);
        var whiteListedTokens = await _mediator.Send(new GetWhitelistedTokensQuery());

        var abi = AbiDefinition.FromJson(Encoding.UTF8.GetString(Resources.hatomControllerAbi));

        foreach (var wtoken in whiteListedTokens)
        {
            var token = tokens.FirstOrDefault(x => x.Identifier == wtoken.TokenIdentifier);

            if (token is null)
            {
                break;
            }

            var usdValueParsed = double.TryParse(token.ValueUSD, NumberStyles.Any, CultureInfo.InvariantCulture, out var valeurDouble);
            var tokenDto = new TokenInContractDto
            {
                TokenIdentifer = token.Identifier,
                MoneyMarket = wtoken?.MoneyMarketAddress,
                HTokenIdentifer = wtoken?.HTokenIdentifier,
                Amount = $"{ESDTAmount.From(token.Balance, ESDT.TOKEN(token.Name, token.Identifier, token.Decimals)).ToCurrencyString(2)} ({valeurDouble:0.00} USD)",
            };

            if (!string.IsNullOrEmpty(wtoken.MoneyMarketAddress))
            {
                var htokenSc = tokens.FirstOrDefault(x => x.Identifier == wtoken.HTokenIdentifier);

                var htoken = await _apiProvider.GetToken(wtoken.HTokenIdentifier);

                var args = new IBinaryType[]
                {
                    Address.FromBech32(wtoken.MoneyMarketAddress),
                    Address.FromBech32(ContractConstants.FACTORING_CONTRACT)
                };

                var result = await SmartContract.QuerySmartContractWithAbiDefinition<NumericValue>(
                        _apiProvider,
                        Address.FromBech32(ContractConstants.HATOM_CONTROLLER_CONTRACT),
                        abi,
                        "getAccountTokens",
                        null,
                        args);

                tokenDto.AmountCollateral = $"{ESDTAmount.From(result.Number, ESDT.TOKEN(htoken.Name, htoken.Identifier, htoken.Decimals)).ToCurrencyString(2)}";
                tokenDto.AmountSupply = $"{ESDTAmount.From(htokenSc?.Balance ?? "0", ESDT.TOKEN(htoken.Name, htoken.Identifier, htoken.Decimals)).ToCurrencyString(2)}";
            }
            tokenInContractDto.Add(tokenDto);
        }

        return tokenInContractDto;
    }
}
