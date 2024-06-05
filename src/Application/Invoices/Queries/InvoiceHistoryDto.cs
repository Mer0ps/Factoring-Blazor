using AutoMapper;
using Domain.Entities;

namespace Application.Invoices.Queries;
public class InvoiceHistoryDto
{
    public long ContractId { get; set; }
    public long InvoiceId { get; set; }
    public string TxHash { get; set; }
    public DateTime TxExecutedAt { get; set; }
    public Status? Status { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<InvoiceHistory, InvoiceHistoryDto>()
                .ForMember(x => x.TxExecutedAt, i => i.MapFrom(map => map.TxExecuteAt));
        }
    }
}
