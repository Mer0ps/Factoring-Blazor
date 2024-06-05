using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Models;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Index
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        [CascadingParameter]
        private DappAccount DappAccount { get; set; }

        protected override void OnInitialized()
        {
        }
    }
}
