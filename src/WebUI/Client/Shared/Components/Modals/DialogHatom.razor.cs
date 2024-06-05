using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using System.Globalization;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;

namespace Mx.Blazor.DApp.Client.Shared.Components.Modals
{
    public partial class DialogHatom
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [ParameterAttribute]
        public DappAccount DappAccount { get; set; }

        private decimal Funds { get; set; }

        [Parameter]
        public DialogHatomType Type { get; set; }
        [Parameter]
        public string TokenIdentifier { get; set; }
        [Parameter]
        public string? MoneyMarket { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += WalletConnected;
        }

        void Cancel() => MudDialog.Cancel();

        private void WalletConnected()
        {
        }

        private async Task Confirm()
        {
            MudDialog.Close(DialogResult.Ok(true));
            switch (Type)
            {
                case DialogHatomType.Mint:
                    await MintToken(MoneyMarket, TokenIdentifier, Funds);
                    break;
                case DialogHatomType.Exit:
                    await ExitMarket(MoneyMarket, Funds, TokenIdentifier);
                    break;
                case DialogHatomType.Redeem:
                    await RedeemToken(MoneyMarket, TokenIdentifier, Funds);
                    break;
                default:
                    break;
            }


            StateHasChanged();
        }

        public async void CancelAction()
        {
            await WalletProvider.CancelAction();
        }

        private async Task MintToken(string moneyMarket, string tokenIdentifier, decimal? amount)
        {

            var tokenDto = await ApiProvider.GetToken(tokenIdentifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            var am = ESDTAmount.ESDT(amount.Value.ToString(CultureInfo.InvariantCulture), esdt);


            var arguments = new IBinaryType[]
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                NumericValue.TokenAmount(am),
                Address.FromBech32(moneyMarket),
            };

            var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(60000000),
                ESDTAmount.Zero(),
                "mintWithUnusedLiquidity",
                arguments.ToArray());

            await WalletProvider.SignAndSendTransaction(transaction, "Mint Token");
        }

        private async Task ExitMarket(string moneyMarket, decimal? amount, string tokenIdentifier)
        {

            var tokenDto = await ApiProvider.GetToken(tokenIdentifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            var am = ESDTAmount.ESDT(amount.Value.ToString(CultureInfo.InvariantCulture), esdt);


            var arguments = new IBinaryType[]
            {
                Address.FromBech32(moneyMarket),
                NumericValue.TokenAmount(am),

            };

            var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(60000000),
                ESDTAmount.Zero(),
                "exitMarketFarm",
                arguments.ToArray());

            await WalletProvider.SignAndSendTransaction(transaction, "Mint Token");
        }

        private async Task RedeemToken(string moneyMarket, string tokenIdentifier, decimal? amount)
        {

            var tokenDto = await ApiProvider.GetToken(tokenIdentifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            var am = ESDTAmount.ESDT(amount.Value.ToString(CultureInfo.InvariantCulture), esdt);


            var arguments = new IBinaryType[]
            {
                ESDTIdentifierValue.From(tokenIdentifier),
                NumericValue.TokenAmount(am),
                Address.FromBech32(moneyMarket),
            };

            var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(40000000),
                ESDTAmount.Zero(),
                "withdrawLiquidity",
                arguments.ToArray());

            await WalletProvider.SignAndSendTransaction(transaction, "Redeem Token");
        }
    }
}
