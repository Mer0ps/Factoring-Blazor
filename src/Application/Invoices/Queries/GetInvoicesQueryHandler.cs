using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Invoices.Queries;

public record GetInvoicesQuery : IRequest<IEnumerable<InvoiceDto>>
{
    public IEnumerable<Status> Status { get; init; }
    public long? AccountId { get; init; }
}

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetInvoicesQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {

        var query = _context.Invoices.AsQueryable();

        if (request.AccountId.HasValue)
        {
            query = query.Where(x => x.Contract.AccountSupplierId == request.AccountId.Value || x.Contract.AccountClientId == request.AccountId.Value);
        }

        if (request.Status != null)
        {
            return await query.Where(invoice => request.Status.Contains(invoice.InvoiceHistories
            .OrderByDescending(history => history.TxExecuteAt)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault().Status.Value))
            .ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        }

        return await query.ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
