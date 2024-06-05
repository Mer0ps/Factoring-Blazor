using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Mx.Blazor.DApp.Client.Shared.Components.Common
{
    public partial class PdfViewer
    {
        [Parameter]
        public string PdfUrl { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("loadPdf", PdfUrl);
            }
        }
    }
}
