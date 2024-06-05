using Blazored.LocalStorage;
using System.Net.Http.Headers;
using static Mx.Blazor.DApp.Client.App.Constants.BrowserLocalStorage;

namespace Mx.Blazor.DApp.Client.Handler;

public class AuthenticatedHttpClientHandler : HttpClientHandler
{
    private readonly ISyncLocalStorageService _localStorage;

    public AuthenticatedHttpClientHandler(ISyncLocalStorageService syncLocalStorageService)
    {
        _localStorage = syncLocalStorageService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = _localStorage.GetItemAsString(ACCESS_TOKEN);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return base.SendAsync(request, cancellationToken);
    }
}
