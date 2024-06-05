using MudBlazor;
using System.Globalization;

namespace Mx.Blazor.DApp.Client.Shared.Components.Chart;

public partial class OverdueChart
{
    public List<ChartSeries> Series = [];
    public string[] XAxisLabels;
    private int Index = -1; //default value cannot be 0 -> first selectedindex is 0.
    public ChartOptions Options = new ChartOptions();

    protected override async Task OnInitializedAsync()
    {
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("us-US");
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("us-US");

        var datas = await FactoringClient.Chart_GetOverdueAsync();

        Series.Add(new ChartSeries { Name = "Overdue", Data = datas.Select(x => Convert.ToDouble(x.NbOverdue)).ToArray() });
        XAxisLabels = datas.Select(x => x.Month).ToArray();
        Options.YAxisTicks = 2;
    }
}
