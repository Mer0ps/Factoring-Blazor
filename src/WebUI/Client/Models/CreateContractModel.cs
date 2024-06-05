using Application.Accounts.Queries;

namespace Mx.Blazor.DApp.Client.Models;

public class CreateContractModel
{
    public CreateContractModel()
    {
        Accounts = Enumerable.Empty<AccountDto>();
    }
    public long? ClientId { get; set; }
    public IEnumerable<AccountDto> Accounts { get; set; }
}
