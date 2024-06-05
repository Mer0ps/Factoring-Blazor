using Application.Invoices.Queries;
using Domain.Constants;
using Domain.Entities;
using Domain.Models;
using MediatR;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.NET.SDK.TransactionsManager;
using Mx.NET.SDK.Wallet;
using Mx.NET.SDK.Wallet.Wallet;
using System.Text.Json;
using Account = Mx.NET.SDK.Domain.Data.Accounts.Account;

namespace Mx.Blazor.DApp.Server.Services;

public class BotService : IBotService
{
    private readonly ApiProvider _apiProvider;
    private readonly IMediator _mediator;
    private readonly Wallet _wallet;

    public BotService(IMediator mediator)
    {
        _apiProvider = new(new ApiNetworkConfiguration(ContractConstants.NETWORK));
        _mediator = mediator;
        _wallet = Wallet.FromPemFile("./Wallet/erd19tyvxrqw9wtzjm0t2r899dllcxd5s7yjms8xp3tahswy9rswcylsa52uxp.pem"); //TODO : Use a keyvault
    }

    public async Task Execute()
    {
        var invoices = await _mediator.Send(new GetInvoicesQuery() { Status = new List<Status> { Status.Valid, Status.Payed } });

        if (invoices.Any())
        {
            var signer = _wallet.GetSigner();

            var networkConfig = await NetworkConfig.GetFromNetwork(_apiProvider);
            var account = new Account(_wallet.GetAddress());
            await account.Sync(_apiProvider);

            var transactions = new List<TransactionRequestDto>();

            foreach (var invoice in invoices)
            {
                var args = new IBinaryType[]
                {
                    NumericValue.U64Value((ulong)invoice.ContractId),
                    NumericValue.U64Value((ulong)invoice.Id),
                };

                var method = invoice.Status == Status.Valid ? "fundInvoice" : "fundRemainingAmount";

                var fundTransaction = EGLDTransactionRequest.EGLDTransferToSmartContract(networkConfig,
                                                        account,
                                                        Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                                                        new GasLimit(10000000),
                                                        ESDTAmount.Zero(),
                                                        method,
                                                        args);

                var signature = signer.SignTransaction(fundTransaction.SerializeForSigning());
                var signedTransaction = fundTransaction.ApplySignature(signature);

                transactions.Add(signedTransaction);

                account.IncrementNonce();

            }

            await _apiProvider.SendTransactions(transactions.ToArray());
        }
    }

    public async Task CalculateScore()
    {
        var signer = _wallet.GetSigner();

        var networkConfig = await NetworkConfig.GetFromNetwork(_apiProvider);
        var account = new Account(_wallet.GetAddress());
        await account.Sync(_apiProvider);

        var fundTransaction = EGLDTransactionRequest.EGLDTransferToSmartContract(networkConfig,
                                                account,
                                                Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                                                new GasLimit(10000000),
                                                ESDTAmount.Zero(),
                                                "calculateReliabilityScore");

        var signature = signer.SignTransaction(fundTransaction.SerializeForSigning());
        var signedTransaction = fundTransaction.ApplySignature(signature);

        account.IncrementNonce();

        await _apiProvider.SendTransaction(signedTransaction);
    }

    public async Task GetEuriborRate()
    {
        var signer = _wallet.GetSigner();

        var networkConfig = await NetworkConfig.GetFromNetwork(_apiProvider);
        var account = new Account(_wallet.GetAddress());
        await account.Sync(_apiProvider);

        var transactions = new List<TransactionRequestDto>();


        //TODO : Hack to find euribor rate, need to find a public API
        var client = new HttpClient();
        string url = "https://www.global-rates.com/highchart-api/?series[0].id=2&series[0].type=2&extra=null";
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();


        string responseBody = await response.Content.ReadAsStringAsync();
        try
        {
            var responses = JsonSerializer.Deserialize<List<Root>>(responseBody);

            foreach (var rates in responses)
            {
                var sortedData = rates.data.OrderBy(d => d[0]).ToList();

                var lastEntry = sortedData.Last();

                long timestamp = Convert.ToInt64(lastEntry[0]) / 1000;

                int preciseRate = (int)Math.Round(lastEntry[1] * ContractConstants.PRECISION_FACTOR);

                var args = new IBinaryType[]
                {
                    NumericValue.U64Value((ulong)timestamp),
                    NumericValue.U32Value((uint)preciseRate),
                };

                var fundTransaction = EGLDTransactionRequest.EGLDTransferToSmartContract(networkConfig,
                                                        account,
                                                        Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                                                        new GasLimit(10000000),
                                                        ESDTAmount.Zero(),
                                                        "addEuriborRate",
                                                        args);

                var signature = signer.SignTransaction(fundTransaction.SerializeForSigning());
                var signedTransaction = fundTransaction.ApplySignature(signature);

                transactions.Add(signedTransaction);

                account.IncrementNonce();


            }

            await _apiProvider.SendTransactions(transactions.ToArray());

        }
        catch (Exception e)
        {
        }
    }
}
