using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.ScAmins.Commands;

public record CreateScAdminCommand : IRequest
{
    public string Address { get; init; }
}

public class CreateScAdminCommandHandler : IRequestHandler<CreateScAdminCommand>
{
    private readonly IFactoringContext _context;

    public CreateScAdminCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateScAdminCommand request, CancellationToken cancellationToken)
    {
        var scAdmin = new ScAdmin
        {
            Address = request.Address,
        };

        _context.ScAdmins.Add(scAdmin);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
