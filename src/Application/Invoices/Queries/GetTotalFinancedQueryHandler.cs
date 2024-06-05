using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Numerics;

namespace Application.Invoices.Queries;
public record GetTotalFinancedQuery : IRequest<IEnumerable<TotalFinancedDto>>
{
    public long? AccountId { get; init; }
}

public class GetTotalFinancedQueryHandler : IRequestHandler<GetTotalFinancedQuery, IEnumerable<TotalFinancedDto>>
{
    private readonly IFactoringContext _context;

    public GetTotalFinancedQueryHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TotalFinancedDto>> Handle(GetTotalFinancedQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var endDate = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

        var startDate = endDate.AddMonths(-12);

        var months = Enumerable.Range(0, 12)
            .Select(i => startDate.AddMonths(i))
            .ToList();

        var query = _context.InvoiceHistories.AsQueryable();

        if (request.AccountId.HasValue)
        {
            query = query.Where(x => x.Invoice.Contract.AccountSupplierId == request.AccountId);
        }

        var groupedInvoices = await query
        .Where(x => x.Status == Domain.Entities.Status.Valid && x.TxExecuteAt >= startDate && x.TxExecuteAt < endDate)
        .GroupBy(x => new { x.TxExecuteAt.Year, x.TxExecuteAt.Month, x.Invoice.Identifier })
        .Select(g => new
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            Identifier = g.Key.Identifier,
            Invoices = g.Select(x => x.Invoice.Amount).ToList()
        })
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .ToListAsync();

        return months.Select(month =>
        {
            var matchingGroup = groupedInvoices.FirstOrDefault(g => g.Year == month.Year && g.Month == month.Month);
            return new TotalFinancedDto
            {
                Month = month.ToString("MMM", new CultureInfo("en-US")),
                Amount = matchingGroup?.Invoices.Aggregate(BigInteger.Zero, (acc, amount) => acc + amount) ?? BigInteger.Zero,
                Identifier = matchingGroup?.Identifier
            };
        }).ToList();
    }
}
