using Domain.Constants;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.App.Constants
{
    public class MultiversxNetwork
    {
        public static ApiProvider Provider
        {
            get => new(new ApiNetworkConfiguration(ContractConstants.NETWORK));
        }

        public static TimeSpan TxCheckTime
        {
            get => TimeSpan.FromSeconds(6);
        }

        public static NetworkConfig NetworkConfig { get; set; } = default!;
        public async static Task InitializeNetworkConfig()
        {
            NetworkConfig ??= await NetworkConfig.GetFromNetwork(Provider);
        }
    }
}