using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.ScAmins.Commands;

public record CreateWhitelistedTokenCommand : IRequest
{
    public string TokenIdentifier { get; init; }
}

public class CreateWhitelistedTokenCommandHandler : IRequestHandler<CreateWhitelistedTokenCommand>
{
    private readonly IFactoringContext _context;

    public CreateWhitelistedTokenCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateWhitelistedTokenCommand request, CancellationToken cancellationToken)
    {
        var token = new WhitelistedToken
        {
            TokenIdentifier = request.TokenIdentifier,
        };

        _context.WhitelistedTokens.Add(token);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
