using Application.Accounts.Queries;
using Mx.NET.SDK.Domain.Data.Accounts;

namespace Mx.Blazor.DApp.Client.Models;

public class DappAccount
{
    public Account Account { get; set; } = default!;
    public AccountDto? AccountDto { get; set; } = default!;
    public bool IsDappAdmin { get; set; } = default!;
}
