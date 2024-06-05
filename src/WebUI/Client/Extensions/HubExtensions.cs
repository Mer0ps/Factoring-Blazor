using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using static Mx.Blazor.DApp.Client.App.Constants.BrowserLocalStorage;

namespace Mx.Blazor.DApp.Client.Extensions;

public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection hubConnection, ISyncLocalStorageService localStorageService)
    {
        if (hubConnection == null)
        {
            var x = localStorageService.GetItemAsString(ACCESS_TOKEN);

            hubConnection = new HubConnectionBuilder()
                              .WithUrl("https://localhost:7187/notificationHub", options =>
                              {
                                  options.AccessTokenProvider = () => Task.FromResult(localStorageService.GetItemAsString(ACCESS_TOKEN));
                              })
                              .WithAutomaticReconnect()
                              .Build();


        }
        return hubConnection;
    }
}
