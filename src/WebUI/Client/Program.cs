using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Domain.Constants;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Mx.Blazor.DApp.Client.Handler;
using Mx.Blazor.DApp.Client.OpenAPI;
using Mx.Blazor.DApp.Client.Services;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.Blazor.DApp.Client.Services.Wallet;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Provider;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddSingleton(new ApiNetworkConfiguration(ContractConstants.NETWORK));
builder.Services.AddSingleton<ApiProvider>();
builder.Services.AddSingleton<AuthenticatedHttpClientHandler>();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddScoped(sp => new HttpClient(new AuthenticatedHttpClientHandler(sp.GetRequiredService<ISyncLocalStorageService>())) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient(new AuthenticatedHttpClientHandler(sp.GetRequiredService<ISyncLocalStorageService>()))
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
    return new FactoringClient(httpClient);
});
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddScoped<PostTxSendService>();
builder.Services.AddScoped<WalletManagerService>();
builder.Services.AddScoped<WalletProviderContainer>();
builder.Services.AddScoped<NativeAuthService>();
builder.Services.AddScoped<TransactionsContainer>();
builder.Services.AddScoped<AccountContainer>();

await builder.Build().RunAsync();
