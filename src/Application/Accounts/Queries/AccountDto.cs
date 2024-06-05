using AutoMapper;
using Domain.Entities;

namespace Application.Accounts.Queries;
public class AccountDto
{
    public AccountDto()
    {
        Administrators = new List<string>();
    }
    public long Id { get; set; }
    public long? OnChainId { get; set; }
    public string CompanyName { get; set; }
    public string VATNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public string WithdrawAddress { get; set; }
    public int? LegalFormId { get; set; }
    public int? BusinessActivityId { get; set; }
    public bool IsKyc { get; set; }
    public int? Score { get; set; }
    public IEnumerable<string> Administrators { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(x => x.Administrators, i => i.MapFrom(map => map.Administrators.Select(account => account.Address)));
        }
    }
}
