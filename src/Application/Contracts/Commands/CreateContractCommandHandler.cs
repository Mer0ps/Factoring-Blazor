using Application.Common.Interfaces;
using MediatR;
using Contract = Domain.Entities.Contract;

namespace Application.Contracts.Commands;

public record CreateContractCommand : IRequest
{
    public long ContractId { get; init; }
    public long SupplierId { get; init; }
    public long ClientId { get; init; }
}

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand>
{
    private readonly IFactoringContext _context;

    public CreateContractCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var contract = new Contract
        {
            Id = request.ContractId,
            AccountSupplierId = request.SupplierId,
            AccountClientId = request.ClientId,
        };

        _context.Contracts.Add(contract);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
