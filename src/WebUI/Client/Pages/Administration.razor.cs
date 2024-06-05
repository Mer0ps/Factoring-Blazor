using Application.ScAmins.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Mx.Blazor.DApp.Client.Extensions;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.SmartContracts;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Administration
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        [CascadingParameter]
        private DappAccount DappAccount { get; set; }

        private AdministratorModel Administrator { get; set; } = new AdministratorModel();
        private WhitelistedTokenModel WhitelistedToken { get; set; } = new WhitelistedTokenModel();
        private HatomAdminModel HatomAdminModel { get; set; } = new HatomAdminModel();

        private IEnumerable<ScAdminDto> admins = new List<ScAdminDto>();
        private IEnumerable<WhitelistedTokenDto> tokens = new List<WhitelistedTokenDto>();

        private HubConnection HubConnection { get; set; }


        private AbiDefinition _abi;

        protected override void OnInitialized()
        {
            TransactionsContainer.TxExecuted += NewTxExecuted;
            TransactionsContainer.HashesExecuted += NewTransactionsExecuted;
        }

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;
            HubConnection = HubConnection.TryInitialize(LocalStorage);

            _abi = AbiDefinition.FromJson(await FactoringClient.Abi_GetAsync("factoringAbi"));
            admins = await FactoringClient.Administrator_GetAllAdminsAsync();

            HubConnection.On(ContractConstants.EVENT_REFRESH_ADMIN, async () =>
            {
                admins = await FactoringClient.Administrator_GetAllAdminsAsync();
                StateHasChanged();
            });

            HubConnection.On(ContractConstants.EVENT_REFRESH_WHITELISTED_TOKEN, async () =>
            {
                tokens = await FactoringClient.Administrator_GetAllWhitelistedTokensAsync();
                StateHasChanged();
            });

            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        public async void NewTxExecuted()
        {
            await DappAccount.Account.Sync(Provider);
            StateHasChanged();
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }

        private async Task HandleAdministratorValidSubmit(EditContext context)
        {
            var arguments = new IBinaryType[]
            {
                Address.FromBech32(Administrator.Address)
            };
            await ManageScAdmin("addUserToAdminList", arguments.ToArray());
        }

        private async Task DeleteAdmin(string address)
        {
            var arguments = new IBinaryType[]
            {
                Address.FromBech32(address)
            };

            await ManageScAdmin("removeUserFromAdminList", arguments.ToArray());
        }

        private async Task HandleTokenValidSubmit(EditContext context)
        {
            var arguments = new IBinaryType[]
            {
                ESDTIdentifierValue.From(WhitelistedToken.TokenIdentifier)
            };

            await ManageScAdmin("addAllowedToken", arguments.ToArray());
        }

        private async Task UpdateToken(WhitelistedTokenDto token)
        {
            await FactoringClient.Administrator_UpdateTokenAsync(token);
        }

        private async Task DeleteToken(string tokenIdentifier)
        {
            var arguments = new IBinaryType[]
            {
                ESDTIdentifierValue.From(tokenIdentifier)
            };

            await ManageScAdmin("removeAllowedToken", arguments.ToArray());
        }

        private async Task HandleHatomAdminSubmit(EditContext context)
        {
            var arguments = new IBinaryType[]
            {
                Address.FromBech32(HatomAdminModel.Address)
            };
            await ManageScAdmin("setHatomControllerAddress", arguments.ToArray());
        }

        private async Task HandleTabChange(int index)
        {
            switch (index)
            {
                case 0:
                    admins = await FactoringClient.Administrator_GetAllAdminsAsync();
                    break;
                case 1:
                    tokens = await FactoringClient.Administrator_GetAllWhitelistedTokensAsync();
                    break;
                case 2:

                    HatomAdminModel.CurrentAddress = (await SmartContract.QuerySmartContractWithAbiDefinition<Address>(
                            ApiProvider,
                            Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                            _abi,
                            "getHatomControllerAddress",
                            null))?.Bech32;
                    break;
                default:
                    break;
            }
        }

        private async Task ManageScAdmin(string method, IBinaryType[] args)
        {

            var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                ESDTAmount.Zero(),
                method,
                args);

            await WalletProvider.SignAndSendTransaction(transaction);
        }
    }
}
