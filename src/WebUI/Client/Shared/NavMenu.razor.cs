using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.Services.Wallet;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }
        [CascadingParameter]
        private DappAccount DappAccount { get; set; }

        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public void Disconnect()
        {
            WalletManagerService.Logout();
        }

        private string GetCompanyName()
        {
            if (!string.IsNullOrEmpty(DappAccount?.AccountDto?.CompanyName))
            {
                return $"- {DappAccount.AccountDto.CompanyName}";
            }
            return string.Empty;
        }

        private Color GetProgressColor(double value)
        {
            if (value == 100)
                return Color.Success; // Vert
            else if (value >= 50 && value < 100)
                return Color.Warning; // Orange
            else
                return Color.Error; // Rouge
        }

        private string GetProgressColorHex(double value)
        {
            if (value == 100)
                return "#4caf50"; // Hex code for green
            else if (value >= 50 && value < 100)
                return "#ff9800"; // Hex code for orange
            else
                return "#f44336"; // Hex code for red
        }
    }
}
