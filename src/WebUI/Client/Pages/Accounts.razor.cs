using Application.Accounts.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages;

public partial class Accounts
{
    [CascadingParameter]
    private DappAccount DappAccount { get; set; }
    private IEnumerable<AccountDto> accounts = new List<AccountDto>();

    private MudTable<AccountDto> table;
    private AccountDto elementBeforeEdit;
    private AccountDto selectedAccount = null;

    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        accounts = await FactoringClient.Account_GetAllAsync();
    }

    private async Task Edit(AccountDto account)
    {
        var administratorsAddress = account.Administrators.Select(x => Address.FromBech32(x)).ToArray();

        var arguments = new IBinaryType[]
        {
            NumericValue.U64Value((ulong)account.Id),
            ListValue.From(TypeValue.AddressValue, administratorsAddress),
            BooleanValue.From(true),
            Address.FromBech32(account.WithdrawAddress)
        };

        var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                ESDTAmount.Zero(),
                "addCompany",
        arguments.ToArray());

        var hash = await WalletProvider.SignAndSendTransaction(transaction, "Validate Account");

        TransactionsContainer.TxExecuted += RefreshScreen;
    }

    private void BackupItem(object element)
    {
        elementBeforeEdit = new()
        {
            IsKyc = ((AccountDto)element).IsKyc,
        };
    }

    private async void RefreshScreen()
    {

    }

    private void ResetItemToOriginalValues(object element)
    {
        ((AccountDto)element).IsKyc = elementBeforeEdit.IsKyc;
    }
}
