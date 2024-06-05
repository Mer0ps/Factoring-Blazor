using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Invoices.Queries;
public record GetInvoiceQuery : IRequest<InvoiceDto?>
{
    public long ContractId { get; init; }
    public long InvoiceId { get; init; }
}

public class GetInvoiceQueryHandler : IRequestHandler<GetInvoiceQuery, InvoiceDto?>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetInvoiceQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
    {
        return await _context.Invoices
            .Where(x => x.Id == request.InvoiceId && x.ContractId == request.ContractId)
            .ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
