using Application.Accounts.Queries;
using Mx.Blazor.DApp.Client.App.Exceptions;
using Mx.Blazor.DApp.Client.Models;
using Mx.NET.SDK.Domain.Data.Accounts;
using static Mx.Blazor.DApp.Client.App.Constants.MultiversxNetwork;

namespace Mx.Blazor.DApp.Client.Services.Containers
{
    public class AccountContainer
    {
        private readonly IHttpService _httpService;

        public AccountContainer(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<DappAccount> Initialize2(string? address)
        {
            var c = new DappAccount();

            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    try
                    {
                        c.AccountDto = await _httpService.GetAsync<AccountDto?>($"api/Account?address={address}");
                    }
                    catch (HttpException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Ne rien faire dans le cas où la ressource n'a pas été trouvée (404)
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    if (c.AccountDto == null)
                    {
                        c.IsDappAdmin = await _httpService.GetAsync<bool>($"api/Account/isDappAdministrator?address={address}");
                    }
                }
            }
            catch (Exception e)
            {

            }

            try
            {
                c.Account = Account.From(await Provider.GetAccount(address));
            }
            catch { }

            return c;

        }

        //public async Task SyncAll()
        //{
        //    await SyncAccount();
        //}

        //public async Task SyncAccount()
        //{
        //    if (Account is null) return;

        //    await Account.Sync(Provider);
        //    AccountDto = await _httpService.GetAsync<AccountDto>($"api/Account?address={Account.Address.Bech32}");
        //}
    }
}
