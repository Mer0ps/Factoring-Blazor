using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Commands;

public record DeleteScAdminCommand : IRequest
{
    public string Address { get; init; }
}

public class DeleteScAdminCommandHandler : IRequestHandler<DeleteScAdminCommand>
{
    private readonly IFactoringContext _context;

    public DeleteScAdminCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteScAdminCommand request, CancellationToken cancellationToken)
    {
        var scAdmin = await _context.ScAdmins
            .Where(l => l.Address == request.Address)
            .SingleOrDefaultAsync(cancellationToken);

        if (scAdmin == null)
        {
            return;
        }

        _context.ScAdmins.Remove(scAdmin);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
