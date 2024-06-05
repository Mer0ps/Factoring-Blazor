using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using System.Numerics;

namespace Application.InvoiceHistories.Commands;

public record CreateCollectedFeeCommand : IRequest
{
    public long InvoiceId { get; init; }
    public long ContractId { get; init; }
    public BigInteger CommissionFee { get; init; }
    public BigInteger FinancingFee { get; init; }
}

public class CreateCollectedFeeCommandHandler : IRequestHandler<CreateCollectedFeeCommand>
{
    private readonly IFactoringContext _context;

    public CreateCollectedFeeCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateCollectedFeeCommand request, CancellationToken cancellationToken)
    {
        var fee = new CollectedFee
        {
            CommissionFee = request.CommissionFee,
            FinancingFee = request.FinancingFee,
            InvoiceId = request.InvoiceId,
            ContractId = request.ContractId
        };

        _context.CollectedFees.Add(fee);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
