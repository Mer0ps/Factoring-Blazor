using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.App.Constants.DAppConstants;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;
using Mx.NET.SDK.NativeAuthClient;
using Mx.NET.SDK.NativeAuthClient.Entities;
using Mx.Blazor.DApp.Client.App.ExtensionMethods;

namespace Mx.Blazor.DApp.Client.Services
{
    public class NativeAuthService
    {
        private readonly NavigationManager _navigationManager;
        private readonly NativeAuthClient _nativeAuthClient;
        public NativeAuthService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _nativeAuthClient = new(new NativeAuthClientConfig()
            {
                Origin = _navigationManager.BaseUri.GetHost(),
                ApiUrl = Provider.NetworkConfiguration.APIUri.AbsoluteUri,
                ExpirySeconds = NATIVE_AUTH_TTL,
                BlockHashShard = 2
            });
        }

        public async Task<string> GenerateToken()
        {
            return await _nativeAuthClient.GenerateToken();
        }
    }
}
