using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Administrators.Commands;

public record CreateAdministratorCommand : IRequest
{
    public long AccountId { get; init; }
    public string Address { get; init; }
}

public class CreateAdministratorCommandHandler : IRequestHandler<CreateAdministratorCommand>
{
    private readonly IFactoringContext _context;

    public CreateAdministratorCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateAdministratorCommand request, CancellationToken cancellationToken)
    {
        var account = _context.Accounts.FirstOrDefaultAsync(x => x.OnChainId == request.AccountId, cancellationToken);

        if (account is null)
        {
            return;
        }

        var scAdmin = new Administrator
        {
            IdAccount = account.Id,
            Address = request.Address,
        };

        _context.Administrators.Add(scAdmin);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
