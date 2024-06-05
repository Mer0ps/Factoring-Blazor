using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Queries;
public record GetScAdminQuery : IRequest<string?>
{
    public string Address { get; init; }
}

public class GetScAdminQueryHandler : IRequestHandler<GetScAdminQuery, string?>
{
    private readonly IFactoringContext _context;

    public GetScAdminQueryHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task<string?> Handle(GetScAdminQuery request, CancellationToken cancellationToken)
    {
        var scAdmin = await _context.ScAdmins
            .Where(x => x.Address == request.Address)
            .FirstOrDefaultAsync(cancellationToken);

        return scAdmin?.Address;
    }
}
