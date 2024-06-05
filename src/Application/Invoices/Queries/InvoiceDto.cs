using AutoMapper;
using Domain.Entities;
using Mx.NET.SDK.Core.Domain;
using System.Numerics;

namespace Application.Invoices.Queries;
public class InvoiceDto
{
    public long Id { get; set; }
    public string Hash { get; set; }
    public BigInteger Amount { get; set; }
    public Status Status { get; set; }
    public long ContractId { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string ClientName { get; set; }
    public int EuriborRate { get; set; }
    public string Identifier { get; set; }
    public ESDT Esdt { get; set; }

    public ESDTAmount ESDTAmount
    {
        get
        {
            return ESDTAmount.From(Amount, Esdt);
        }
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(x => x.Status, i => i.MapFrom(map => map.InvoiceHistories.Where(x => x.Status != null).OrderByDescending(x => x.CreatedAt).Select(x => x.Status).FirstOrDefault() ?? Status.PendingValidation))
                .ForMember(x => x.SupplierId, i => i.MapFrom(map => map.Contract.AccountSupplierId))
                .ForMember(x => x.SupplierName, i => i.MapFrom(map => map.Contract.AccountSupplier.CompanyName))
                .ForMember(x => x.ClientName, i => i.MapFrom(map => map.Contract.AccountClient.CompanyName));
        }
    }
}
