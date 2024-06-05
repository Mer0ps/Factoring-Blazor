using Application.Common.Interfaces;
using MediatR;

namespace Application.Contracts.Commands;

public record SignContractCommand : IRequest<SignContractResultDto?>
{
    public long ContractId { get; init; }
}

public record SignContractResultDto
{
    public long SupplierId { get; init; }
    public long ClientId { get; init; }
}

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, SignContractResultDto?>
{
    private readonly IFactoringContext _context;

    public SignContractCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task<SignContractResultDto?> Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts.FindAsync(request.ContractId, cancellationToken);

        if (contract is null)
        {
            return null;
        }

        contract.IsSigned = true;

        await _context.SaveChangesAsync(cancellationToken);

        return new SignContractResultDto
        {
            SupplierId = contract.AccountSupplierId,
            ClientId = contract.AccountClientId,
        };
    }
}
