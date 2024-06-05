using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using System.Numerics;

namespace Application.Invoices.Commands;

public record CreateInvoiceCommand : IRequest
{
    public long InvoiceId { get; init; }

    public long ContractId { get; init; }
    public string Hash { get; init; }
    public string Identifier { get; init; }
    public BigInteger Amount { get; init; }
    public DateTime DueDate { get; init; }
    public string TxHash { get; init; }
    public DateTime TxExecuteAt { get; init; }
    public int EuriborRate { get; init; }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand>
{
    private readonly IFactoringContext _context;

    public CreateInvoiceCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice
        {
            Id = request.InvoiceId,
            ContractId = request.ContractId,
            Hash = request.Hash,
            Identifier = request.Identifier,
            Amount = request.Amount,
            DueDate = request.DueDate,
            EuriborRate = request.EuriborRate,
        };

        var invoiceHistory = new InvoiceHistory
        {
            InvoiceId = request.InvoiceId,
            ContractId = request.ContractId,
            Status = Status.PendingValidation,
            TxHash = request.TxHash,
            TxExecuteAt = request.TxExecuteAt,
        };

        _context.Invoices.Add(invoice);
        _context.InvoiceHistories.Add(invoiceHistory);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
