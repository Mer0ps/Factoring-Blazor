using AutoMapper;
using Domain.Entities;

namespace Application.Contracts.Queries;

public class ContractDto
{
    public long Id { get; set; }
    public long? IdNullable { get { return Id; } }
    public long IdSupplier { get; set; }
    public string? SupplierName { get; set; }
    public string? ClientName { get; set; }
    public long IdClient { get; set; }
    public bool IsSigned { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Contract, ContractDto>()
                .ForMember(x => x.IdClient, i => i.MapFrom(map => map.AccountClientId))
                .ForMember(x => x.ClientName, i => i.MapFrom(map => map.AccountClient.CompanyName))
                .ForMember(x => x.IdSupplier, i => i.MapFrom(map => map.AccountSupplierId))
                .ForMember(x => x.SupplierName, i => i.MapFrom(map => map.AccountSupplier.CompanyName));
        }
    }
}
