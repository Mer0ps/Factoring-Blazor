using Application.ScAmins.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using System.Globalization;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.TokenTransactionRequest;

namespace Mx.Blazor.DApp.Client.Shared.Components.Modals
{
    public partial class AddFunds
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [ParameterAttribute]
        public DappAccount DappAccount { get; set; }

        private decimal Funds { get; set; }
        private string TokenIdentifier { get; set; }
        private IEnumerable<WhitelistedTokenDto> Tokens { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += WalletConnected;
        }

        protected override async Task OnInitializedAsync()
        {
            Tokens = await FactoringClient.Administrator_GetAllWhitelistedTokensAsync();
        }

        void Cancel() => MudDialog.Cancel();

        private void WalletConnected()
        {
        }

        private async Task Add()
        {
            var tokenDto = await ApiProvider.GetToken(TokenIdentifier);
            var am = ESDTAmount.ESDT(Funds.ToString(CultureInfo.InvariantCulture), ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals));


            var arguments = new IBinaryType[]
            {
                NumericValue.U64Value((ulong)DappAccount.AccountDto.OnChainId.Value)
            };

            var tokenIdentifier = ESDTIdentifierValue.From(TokenIdentifier);
            var transaction = TokenTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                tokenIdentifier,
                am,
                "addAccountFunds",
                arguments);

            MudDialog.Close(DialogResult.Ok(true));

            var hash = await WalletProvider.SignAndSendTransaction(transaction, "Add funds");

            StateHasChanged();
        }

        public async void CancelAction()
        {
            await WalletProvider.CancelAction();
        }
    }
}
