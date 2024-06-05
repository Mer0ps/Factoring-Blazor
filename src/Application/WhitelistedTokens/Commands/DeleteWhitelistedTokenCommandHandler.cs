using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Commands;

public record DeleteWhitelistedTokenCommand : IRequest
{
    public string TokenIdentifier { get; init; }
}

public class DeleteWhitelistedTokenCommandHandler : IRequestHandler<DeleteWhitelistedTokenCommand>
{
    private readonly IFactoringContext _context;

    public DeleteWhitelistedTokenCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteWhitelistedTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _context.WhitelistedTokens
            .Where(l => l.TokenIdentifier == request.TokenIdentifier)
            .SingleOrDefaultAsync(cancellationToken);

        if (token == null)
        {
            return;
        }

        _context.WhitelistedTokens.Remove(token);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
