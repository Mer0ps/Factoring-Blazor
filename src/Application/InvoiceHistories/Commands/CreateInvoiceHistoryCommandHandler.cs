using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.InvoiceHistories.Commands;

public record CreateInvoiceHistoryCommand : IRequest
{
    public long InvoiceId { get; init; }
    public long ContractId { get; init; }
    public Status Status { get; init; }
    public string TxHash { get; init; }
    public DateTime TxExecuteAt { get; init; }
}

public class CreateInvoiceHistoryCommandHandler : IRequestHandler<CreateInvoiceHistoryCommand>
{
    private readonly IFactoringContext _context;

    public CreateInvoiceHistoryCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateInvoiceHistoryCommand request, CancellationToken cancellationToken)
    {
        var invoiceHistory = new InvoiceHistory
        {
            InvoiceId = request.InvoiceId,
            ContractId = request.ContractId,
            Status = request.Status,
            TxHash = request.TxHash,
            TxExecuteAt = request.TxExecuteAt,
        };

        _context.InvoiceHistories.Add(invoiceHistory);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
