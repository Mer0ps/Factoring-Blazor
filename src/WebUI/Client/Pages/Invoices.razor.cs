using Application.Invoices.Queries;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Mx.Blazor.DApp.Client.Extensions;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;
using static Mx.NET.SDK.TransactionsManager.TokenTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages;

public partial class Invoices : IDisposable
{

    [CascadingParameter]
    private DappAccount DappAccount { get; set; }
    private IEnumerable<InvoiceDto> invoices = new List<InvoiceDto>();
    private AbiDefinition _abi;
    private bool _loading;
    private HubConnection HubConnection { get; set; }
    protected override void OnInitialized()
    {
        TransactionsContainer.TxExecuted += NewTxExecuted;
    }
    protected override async Task OnInitializedAsync()
    {
        invoices = await FactoringClient.Invoice_GetAllAsync();

        HubConnection = HubConnection.TryInitialize(LocalStorage);

        HubConnection.On(ContractConstants.EVENT_REFRESH_INVOICE, async () =>
        {
            invoices = await FactoringClient.Invoice_GetAllAsync();
            StateHasChanged();
        });

        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }

    }
    public void Dispose()
    {
        HubConnection.Remove(ContractConstants.EVENT_REFRESH_INVOICE);
    }

    public async void NewTxExecuted()
    {
        await DappAccount.Account.Sync(Provider);
        StateHasChanged();
    }
    private async Task Confirm(InvoiceDto invoice, Status status)
    {

        var arguments = new IBinaryType[]
        {
            NumericValue.U64Value((ulong)invoice.ContractId),
            NumericValue.U64Value((ulong)invoice.Id),
            new NumericValue(TypeValue.U8TypeValue, (int)status)
        };

        var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                ESDTAmount.Zero(),
                "confirmInvoice",
        arguments.ToArray());

        var hash = await WalletProvider.SignAndSendTransaction(transaction, "Confirm invoice status");

        TransactionsContainer.TxExecuted += RefreshScreen;
    }

    private async Task Pay(InvoiceDto invoice)
    {

        var arguments = new IBinaryType[]
        {
            NumericValue.U64Value((ulong)invoice.ContractId),
            NumericValue.U64Value((ulong)invoice.Id),
        };

        var tokenIdentifier = ESDTIdentifierValue.From(invoice.Esdt.Identifier);
        var transaction = TokenTransferToSmartContract(
            NetworkConfig,
            DappAccount.Account,
            Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
            new GasLimit(10000000),
            tokenIdentifier,
            invoice.ESDTAmount,
            "payInvoice",
            arguments.ToArray());

        var hash = await WalletProvider.SignAndSendTransaction(transaction, "Pay Invoice");
    }

    public void RowClicked(TableRowClickEventArgs<InvoiceDto> p)
    {
        NavigationManager.NavigateTo($"/Invoice/{p.Item.ContractId}/{p.Item.Id}");
    }
    private async void RefreshScreen()
    {

    }
}
