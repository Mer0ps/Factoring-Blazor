using Infrastructure.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using System.Globalization;

namespace Mx.Blazor.DApp.Client.Shared.Components.Chart;

public partial class TotalFinancedChart
{
    [CascadingParameter]
    private DappAccount DappAccount { get; set; }
    public List<ChartSeries> Series = [];
    public string[] XAxisLabels;
    private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
    public ChartOptions Options = new ChartOptions();

    protected override async Task OnInitializedAsync()
    {
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("us-US");
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("us-US");

        long? accountId = null;

        if (DappAccount != null)
        {
            accountId = DappAccount.IsDappAdmin ? null : DappAccount.AccountDto?.Id;
        }

        var datas = await FactoringClient.Chart_GetTotalFinancedAsync(accountId);

        Series.Add(new ChartSeries { Name = "Amount financed", Data = datas.Select(x => x.Amount.AmountToDouble(x.Esdt)).ToArray() });
        XAxisLabels = datas.Select(x => x.Month).ToArray();
        Options.YAxisFormat = "C";
    }
}
