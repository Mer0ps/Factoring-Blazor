using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Configuration;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class MainLayout
    {
        private bool WalletConnected;
        private DappAccount? DappAccount;
        private string CurrentNetwork { get; set; } = "";
        private bool _isDarkMode = true;
        private bool _isInitialized = false;

        public MainLayout()
        {
            switch (Provider.NetworkConfiguration.Network)
            {
                case Network.MainNet:
                    CurrentNetwork = "MainNet";
                    break;
                case Network.DevNet:
                    CurrentNetwork = "DevNet";
                    break;
                case Network.TestNet:
                    CurrentNetwork = "TestNet";
                    break;
            }
        }

        protected override void OnInitialized()
        {
            WalletConnected = WalletProvider.IsConnected();

            WalletProvider.OnWalletConnected += OnWalletConnected;
            WalletProvider.OnWalletDisconnected += OnWalletDisconnected;
        }

        protected override async Task OnInitializedAsync()
        {
            await WalletProvider.InitializeAsync();

            if (WalletConnected)
                await InitializeConnection();

            var address = WalletProvider.GetAddress();

            if (address != null)
            {
                //await AccountContainer.Initialize(address);
                DappAccount = await AccountContainer.Initialize2(address);
            }

            _isInitialized = true;

        }

        private async Task InitializeConnection()
        {
            await InitializeNetworkConfig();
            await WalletManager.InitializeAsync();
        }

        private async void OnWalletConnected()
        {
            WalletConnected = WalletProvider.IsConnected();
            var address = WalletProvider.GetAddress();
            //await AccountContainer.Initialize(address);

            DappAccount = await AccountContainer.Initialize2(address);

            StateHasChanged();

            await InitializeConnection();
        }

        private void OnWalletDisconnected()
        {
            WalletConnected = WalletProvider.IsConnected();
            DappAccount = null;
            NavigationManager.NavigateTo("");
            StateHasChanged();
        }
    }
}
