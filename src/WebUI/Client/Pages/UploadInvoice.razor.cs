using Domain.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Domain.Helper;
using System.Globalization;
using System.Net.Http.Headers;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using static Mx.NET.SDK.TransactionsManager.EGLDTransactionRequest;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class UploadInvoice
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }
        [CascadingParameter]
        private DappAccount DappAccount { get; set; }

        private InvoiceModel InvoiceModel { get; set; }

        private MudForm form;
        private bool SuppressOnChangeWhenInvalid;

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += OnWalletConnected;
            TransactionsContainer.HashesExecuted += NewTransactionsExecuted;
            InvoiceModel = new InvoiceModel();
        }

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;

            InvoiceModel.Contracts = await FactoringClient.Contract_GetContractsForSupplierAsync(DappAccount.AccountDto.OnChainId.Value);
            InvoiceModel.TokenIdentifiers = await FactoringClient.Administrator_GetAllWhitelistedTokensAsync();

            await DisplayAccountInformation();
        }

        private async void OnWalletConnected()
        {
            await DisplayAccountInformation();
        }

        public async Task DisplayAccountInformation()
        {
            var address = WalletProvider.GetAddress();
            //await AccountContainer.Initialize(address);

            StateHasChanged();
        }

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                long maxFileSize = 1024 * 15000;
                using var content = new MultipartFormDataContent();

                var fileContent =
                            new StreamContent(InvoiceModel.File.OpenReadStream(maxFileSize));

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(InvoiceModel.File.ContentType);

                content.Add(
                    content: fileContent,
                    name: "invoice",
                    fileName: InvoiceModel.File.Name);


                var result = await Http.PostHttpContentAsync<string>("api/Invoice/uploadToIpfs", content); //TODO refacto

                var tokenDto = await ApiProvider.GetToken(InvoiceModel.TokenIdentifier);
                var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
                var am = ESDTAmount.ESDT(InvoiceModel.Amount.ToString(CultureInfo.InvariantCulture), esdt);

                var arguments = new IBinaryType[]
                {
                    NumericValue.U64Value((ulong)InvoiceModel.IdContract),
                    BytesValue.FromUtf8(result),
                    NumericValue.TokenAmount(am),
                    NumericValue.U64Value((ulong)InvoiceModel.IssueDate.Value.ToTimestamp()),
                    NumericValue.U64Value((ulong)InvoiceModel.DueDate.Value.ToTimestamp()),
                    ESDTIdentifierValue.From(InvoiceModel.TokenIdentifier)
                };

                var transaction = EGLDTransferToSmartContract(
                        NetworkConfig,
                        DappAccount.Account,
                        Address.FromBech32(ContractConstants.FACTORING_CONTRACT),
                        new GasLimit(10000000),
                        ESDTAmount.Zero(),
                        "addInvoice",
                arguments.ToArray());

                var hash = await WalletProvider.SignAndSendTransaction(transaction, "Upload Invoice");

            }
        }

        public void NewTransactionsExecuted(string[] hashes)
        {
            //do something
        }
    }
}
