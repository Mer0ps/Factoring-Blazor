using Application.Contracts.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages;

public partial class Contracts
{
    [CascadingParameter]
    private bool WalletConnected { get; set; }
    [CascadingParameter]
    private DappAccount DappAccount { get; set; }
    private IEnumerable<ContractDto> contractsClient = new List<ContractDto>();
    private IEnumerable<ContractDto> contractsSupplier = new List<ContractDto>();
    private bool _loading;
    private CreateContractModel AccountModel { get; set; }

    private int activeIndex { get; set; }

    protected override void OnInitialized()
    {
        AccountModel = new CreateContractModel();
    }

    protected override async Task OnInitializedAsync()
    {
        var allAccounts = await FactoringClient.Account_GetAllAsync();
        AccountModel.Accounts = allAccounts.Where(x => x.OnChainId.HasValue && x.OnChainId.Value != DappAccount.AccountDto.OnChainId.Value).ToList();
    }

    private async Task Edit(ContractDto contract)
    {

        var arguments = new IBinaryType[]
        {
            NumericValue.U64Value((ulong)contract.Id)
        };

        var transaction = EGLDTransferToSmartContract(
                NetworkConfig,
                DappAccount.Account,
                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                new GasLimit(10000000),
                ESDTAmount.Zero(),
                "signContract",
        arguments.ToArray());

        var hash = await WalletProvider.SignAndSendTransaction(transaction, "Validate Account");
    }

    public async Task CreateContrat()
    {
        if (AccountModel.ClientId.HasValue)
        {
            var arguments = new IBinaryType[]
            {
                    NumericValue.U64Value((ulong)DappAccount.AccountDto.OnChainId.Value),
                    NumericValue.U64Value((ulong)AccountModel.ClientId.Value),
            };

            var transaction = EGLDTransferToSmartContract(
                    NetworkConfig,
                    DappAccount.Account,
                    Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                    new GasLimit(10000000),
                    ESDTAmount.Zero(),
                    "createFactoringContract",
            arguments.ToArray());

            var hash = await WalletProvider.SignAndSendTransaction(transaction, "Create contract");
        }
    }

    async Task TabChanged(int index)
    {
        switch (index)
        {
            case 1:
                contractsClient = await FactoringClient.Contract_GetContractsClientAsync(DappAccount?.AccountDto?.Id);
                break;
            case 2:
                contractsSupplier = await FactoringClient.Contract_GetContractsSupplierAsync(DappAccount?.AccountDto?.Id);
                break;
            default:
                break;
        }
    }

}
