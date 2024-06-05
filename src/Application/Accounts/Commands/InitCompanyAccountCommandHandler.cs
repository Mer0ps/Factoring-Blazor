using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Commands;
public record InitCompanyAccountCommand : IRequest
{
    public long AccountIdOffChain { get; init; }
    public long AccountId { get; init; }
    public int Score { get; init; }
    public int Fee { get; init; }
}

public class InitCompanyAccountCommandHandler : IRequestHandler<InitCompanyAccountCommand>
{
    private readonly IFactoringContext _context;

    public InitCompanyAccountCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(InitCompanyAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountIdOffChain, cancellationToken);

        if (account == null)
        {
            return;
        }

        account.IsKyc = true;
        account.OnChainId = request.AccountId;
        account.Score = request.Score;
        account.Fee = request.Fee / 100;

        await _context.SaveChangesAsync(cancellationToken);
    }
}