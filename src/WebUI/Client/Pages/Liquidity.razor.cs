using Application.WhitelistedTokens.Queries;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.Blazor.DApp.Client.Shared.Components.Modals;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.SmartContracts;
using System.Globalization;
using System.Numerics;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.TokenTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Liquidity
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        [CascadingParameter]
        private DappAccount DappAccount { get; set; }
        private FundsModel Funds { get; set; } = new FundsModel();
        private HatomModel HatomModel { get; set; } = new HatomModel();
        private IEnumerable<TokenInContractDto> tokens = new List<TokenInContractDto>();
        private AbiDefinition _factoringAbi;
        private AbiDefinition _hatomControllerAbi;
        private int Tab = 0;



        protected override void OnInitialized()
        {
            TransactionsContainer.TxExecuted += NewTxExecuted;
            TransactionsContainer.HashesExecuted += NewTransactionsExecuted;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;
            Funds.TokenIdentifiers = await FactoringClient.Administrator_GetAllWhitelistedTokensAsync();

            _factoringAbi = AbiDefinition.FromJson(await FactoringClient.Abi_GetAsync("factoringAbi"));
            _hatomControllerAbi = AbiDefinition.FromJson(await FactoringClient.Abi_GetAsync("hatomControllerAbi"));
        }

        private async Task HandleAddFundsValidSubmit(EditContext context)
        {

            var am = ESDTAmount.ESDT(Funds.Amount.Value.ToString(CultureInfo.InvariantCulture), Funds.TokenESDT);


            TransactionRequest transaction;

            if (Funds.IsAdd)
            {
                var tokenIdentifier = ESDTIdentifierValue.From(Funds.TokenIdentifier);
                transaction = TokenTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                tokenIdentifier,
                am,
                "addProcolFunds");
            }
            else
            {
                var arguments = new IBinaryType[]
                {
                    ESDTIdentifierValue.From(Funds.TokenIdentifier),
                    NumericValue.TokenAmount(am),
                };

                transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                ESDTAmount.Zero(),
                "removeProcolFunds",
                arguments.ToArray());
            }

            var hash = await WalletProvider.SignAndSendTransaction(transaction, "Add funds");

            Funds.Amount = null;

            StateHasChanged();
        }

        private async Task HandleTokenIdentifierChanged(IEnumerable<string> identifiers)
        {
            var identifier = identifiers.FirstOrDefault();

            var tokenDto = await ApiProvider.GetToken(identifier);
            Funds.TokenESDT = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);

            if (identifier == null) return;
            Funds.CurrentAmountInSc = await GetCurrentFund(identifier);
        }

        private async Task<string?> GetCurrentFund(string? identifier)
        {
            var args = new IBinaryType[]
                        {
                ESDTIdentifierValue.From(identifier)
                        };

            var result = await SmartContract.QuerySmartContractWithAbiDefinition<NumericValue>(
                    ApiProvider,
                    Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                    _factoringAbi,
                    "getProtocolFunds",
                    null,
                    args);

            return result != null ? ESDTAmount.From(result.ToObject<BigInteger>(), Funds.TokenESDT).ToCurrencyString() : null;
        }

        private async Task ClaimRewards()
        {

            var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(200000000),
                ESDTAmount.Zero(),
                "claimFarmingRewards");

            var hash = await WalletProvider.SignAndSendTransaction(transaction, "Claim rewards");

        }

        private async Task HandleTabChange(int index)
        {
            Tab = index;
            switch (index)
            {
                case 1:
                    tokens = await FactoringClient.Administrator_GetScTokensAsync();

                    break;
                default:
                    break;
            }
        }

        public async void NewTxExecuted()
        {
            await DappAccount.Account.Sync(Provider);


            if (Tab == 1)
            {
                tokens = await FactoringClient.Administrator_GetScTokensAsync();
            }
            else if (Funds.TokenIdentifier is not null)
            {
                Funds.CurrentAmountInSc = Funds.CurrentAmountInSc = await GetCurrentFund(Funds.TokenIdentifier);
            }
            StateHasChanged();
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }

        private void OpenDialog(DialogHatomType type, string tokenIdentifier, string? moneyMarket)
        {
            var parameters = new DialogParameters<DialogHatom>
            {
                { x => x.Type, type },
                { x => x.TokenIdentifier, tokenIdentifier },
                { x => x.MoneyMarket, moneyMarket },
                { x => x.DappAccount, DappAccount },
            };
            Dialog.Show<DialogHatom>(type.ToString(), parameters);
        }
    }
}
