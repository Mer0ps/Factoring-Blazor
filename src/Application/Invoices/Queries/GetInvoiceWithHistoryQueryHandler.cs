using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Invoices.Queries;
public record GetInvoiceWithHistoryQuery : IRequest<InvoiceDetailDto>
{
    public long ContractId { get; init; }
    public long InvoiceId { get; init; }
}

public class GetInvoiceWithHistoryQueryHandler : IRequestHandler<GetInvoiceWithHistoryQuery, InvoiceDetailDto>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetInvoiceWithHistoryQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceDetailDto> Handle(GetInvoiceWithHistoryQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Where(x => x.Id == request.InvoiceId && x.ContractId == request.ContractId)
            .ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        var histories = await _context.InvoiceHistories
            .Where(x => x.ContractId == request.ContractId && x.InvoiceId == request.InvoiceId)
            .ProjectTo<InvoiceHistoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new InvoiceDetailDto
        {
            Invoice = invoice,
            Histories = histories
        };
    }
}
