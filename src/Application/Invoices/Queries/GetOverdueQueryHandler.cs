using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Invoices.Queries;
public record GetOverduedQuery : IRequest<IEnumerable<OverdueDto>>
{
}

public class GetOverdueQueryHandler : IRequestHandler<GetOverduedQuery, IEnumerable<OverdueDto>>
{
    private readonly IFactoringContext _context;

    public GetOverdueQueryHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OverdueDto>> Handle(GetOverduedQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Today.ToUniversalTime();
        var endDate = DateTime.SpecifyKind(new DateTime(today.Year, today.Month, 1), DateTimeKind.Utc);

        var startDate = endDate.AddMonths(-12);

        var months = Enumerable.Range(0, 12)
            .Select(i => startDate.AddMonths(i))
            .ToList();

        var groupedInvoices = await _context.Invoices
        .Where(x => x.DueDate >= startDate && x.DueDate < today && !x.InvoiceHistories.Any(x => x.Status == Domain.Entities.Status.Payed))
        .GroupBy(x => new { x.DueDate.Year, x.DueDate.Month })
        .Select(g => new
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            NbOverdue = g.Count()
        })
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .ToListAsync();

        return months.Select(month =>
        {
            var matchingGroup = groupedInvoices.FirstOrDefault(g => g.Year == month.Year && g.Month == month.Month);
            return new OverdueDto
            {
                Month = month.ToString("MMM", new CultureInfo("en-US")),
                NbOverdue = matchingGroup?.NbOverdue ?? 0,
            };
        }).ToList();
    }
}
