using Domain.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.Blazor.DApp.Client.Shared.Components.Modals;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.SmartContracts;
using System.Numerics;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Dashboard
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }
        [CascadingParameter]
        public DappAccount DappAccount { get; set; }
        private AbiDefinition _abi;

        private string FundsInSc { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += OnWalletConnected;
            TransactionsContainer.HashesExecuted += NewTransactionsExecuted;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;

            _abi = AbiDefinition.FromJson(await FactoringClient.Abi_GetAsync("factoringAbi"));

            await LoadScFunds();
        }

        private async void OnWalletConnected()
        {
        }

        private async Task OpenDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            DialogParameters parameters = new()
            {
                { "DappAccount", DappAccount }
            };
            var dialog = Dialog.Show<AddFunds>("Preload my funds", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadScFunds();
            }
        }

        private async Task LoadScFunds()
        {
            var tokenIdentifier = "USDC-350c4e"; // TODO : tmp hack
            var tokenDto = await ApiProvider.GetToken(tokenIdentifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            var args = new IBinaryType[]
            {
                NumericValue.U64Value((ulong)DappAccount.AccountDto.OnChainId.Value),
                ESDTIdentifierValue.From(tokenIdentifier)
            };

            var result = await SmartContract.QuerySmartContractWithAbiDefinition<NumericValue>(
                    ApiProvider,
                    Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                    _abi,
                    "getFundsByAccount", null, args
                    );

            FundsInSc = ESDTAmount.From(result.ToObject<BigInteger>(), esdt).ToCurrencyString();
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }
    }
}
