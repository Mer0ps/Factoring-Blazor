using Application.BusinessActivities.Queries;
using Application.LegalForms.Queries;
using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Models;
using Mx.Blazor.DApp.Client.OpenAPI;
using Mx.Blazor.DApp.Client.Services.Containers;
using AccountDto = Application.Accounts.Queries.AccountDto;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Account
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        [CascadingParameter]
        private DappAccount DappAccount { get; set; }

        private AccountDto AccountDto { get; set; } = new AccountDto();
        private IEnumerable<LegalFormDto> LegalForms { get; set; } = new List<LegalFormDto>();
        private IEnumerable<LegalFormDto> LegalFormSelectList { get; set; } = new List<LegalFormDto>();
        private IEnumerable<BusinessActivityDto> BusinessActivitiesSelectList { get; set; } = new List<BusinessActivityDto>();

        protected override async Task OnInitializedAsync()
        {
            if (!WalletConnected) return;

            BusinessActivitiesSelectList = await FactoringClient.Account_GetBusinessActivitiesAsync();
            LegalForms = await FactoringClient.Account_GetLegalFormsAsync();
        }


        public async Task HandleCreateAccountValidSubmit()
        {

            await FactoringClient.Account_CreateAccountAsync(AccountDto);

            var address = WalletProvider.GetAddress();

            if (address != null)
            {
                DappAccount = await AccountContainer.Initialize2(address);

                NavigationManager.NavigateTo("", true);
                StateHasChanged();
            }
        }

        private void HandleVATChanged()
        {
            var countryCode2 = GetCountryCode(AccountDto.VATNumber);
            LegalFormSelectList = LegalForms.Where(x => x.CountryCode == countryCode2);
        }

        public static string GetCountryCode(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 2)
            {
                return string.Empty;
            }
            return input.Substring(0, 2);
        }
    }
}
