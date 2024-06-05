using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Commands;

public record UpdateAccountScoreCommand : IRequest
{
    public int Score { get; init; }
    public long AccountIdOffChain { get; init; }
}

public class UpdateAccountScoreCommandHandler : IRequestHandler<UpdateAccountScoreCommand>
{
    private readonly IFactoringContext _context;

    public UpdateAccountScoreCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAccountScoreCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == request.AccountIdOffChain, cancellationToken);

        if (account == null)
        {
            return;
        }

        account.Score = request.Score;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
