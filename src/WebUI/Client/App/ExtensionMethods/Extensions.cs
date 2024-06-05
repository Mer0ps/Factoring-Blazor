using Blazored.LocalStorage;
using static Mx.Blazor.DApp.Client.App.Constants.BrowserLocalStorage;

namespace Mx.Blazor.DApp.Client.App.ExtensionMethods
{
    public static class Extensions
    {
        public static void RemoveAllWcItems(this ISyncLocalStorageService localStorage)
        {
            localStorage.RemoveItem(WALLET_CONNECT_DEF_STORAGE);
            localStorage.RemoveItems(localStorage.Keys().Where(k => k.StartsWith("wc@2")));
        }
    }
}
