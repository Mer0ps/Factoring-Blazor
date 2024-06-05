using Application.Common.Interfaces;
using MediatR;

namespace Application.ScAmins.Commands;

public record UpdateWhitelistedTokenCommand : IRequest
{
    public string TokenIdentifier { get; init; }
    public string? MoneyMarketAddress { get; init; }
    public string? HTokenIdentifier { get; init; }
}

public class UpdateWhitelistedTokenCommandHandler : IRequestHandler<UpdateWhitelistedTokenCommand>
{
    private readonly IFactoringContext _context;

    public UpdateWhitelistedTokenCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateWhitelistedTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _context.WhitelistedTokens.FindAsync(request.TokenIdentifier, cancellationToken);

        if (token == null)
        {
            return;
        }

        token.MoneyMarketAddress = request.MoneyMarketAddress;
        token.HTokenIdentifier = request.HTokenIdentifier;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
