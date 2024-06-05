using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Numerics;

namespace Application.Invoices.Queries;
public record GetTotalCollectedFeeQuery : IRequest<IEnumerable<TotalCollectedFeeDto>>
{
    public long? AccountId { get; init; }
}

public class GetTotalCollectedFeeQueryHandler : IRequestHandler<GetTotalCollectedFeeQuery, IEnumerable<TotalCollectedFeeDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetTotalCollectedFeeQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TotalCollectedFeeDto>> Handle(GetTotalCollectedFeeQuery request, CancellationToken cancellationToken)
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

        var groupedInvoices = await query.Where(x => x.Status == Status.FullyFunded && x.TxExecuteAt >= startDate && x.TxExecuteAt < endDate)
        .GroupBy(x => new { x.TxExecuteAt.Year, x.TxExecuteAt.Month, x.Invoice.Identifier })
        .Select(g => new
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            Identifier = g.Key.Identifier,
            CommissionFee = g.Select(x => x.Invoice.CollectedFees.FirstOrDefault().CommissionFee).ToList(),
            FinancingFee = g.Select(x => x.Invoice.CollectedFees.FirstOrDefault().FinancingFee).ToList()
        })
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .ToListAsync();

        return months.Select(month =>
        {
            var matchingGroup = groupedInvoices.FirstOrDefault(g => g.Year == month.Year && g.Month == month.Month);
            return new TotalCollectedFeeDto
            {
                Month = month.ToString("MMM", new CultureInfo("en-US")),
                Amount = matchingGroup?.FinancingFee.Aggregate(BigInteger.Zero, (acc, amount) => acc + amount) ?? BigInteger.Zero,
                Amount2 = matchingGroup?.CommissionFee.Aggregate(BigInteger.Zero, (acc, amount) => acc + amount) ?? BigInteger.Zero,
                Identifier = matchingGroup?.Identifier
            };
        }).ToList();
    }
}
