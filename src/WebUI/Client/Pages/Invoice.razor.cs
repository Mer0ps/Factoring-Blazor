using Application.Invoices.Queries;
using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Models;

namespace Mx.Blazor.DApp.Client.Pages;

public partial class Invoice
{

    [CascadingParameter]
    private DappAccount DappAccount { get; set; }
    [Parameter]
    public long IdContract { get; set; }
    [Parameter]
    public long IdInvoice { get; set; }

    private InvoiceDetailDto InvoiceDetail { get; set; }

    protected override async Task OnInitializedAsync()
    {
        InvoiceDetail = await FactoringClient.Invoice_GetInvoiceAsync(IdContract, IdInvoice);
    }

}
