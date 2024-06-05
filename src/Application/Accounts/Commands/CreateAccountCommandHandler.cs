using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Accounts.Commands;

public record CreateAccountCommand : IRequest
{
    public string CompanyName { get; init; }
    public string VATNumber { get; init; }
    public string RegistrationNumber { get; init; }
    public string WithdrawAddress { get; init; }
    public string Caller { get; init; }
    public int LegalFormId { get; init; }
    public int BusinessActivityId { get; init; }
}

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand>
{
    private readonly IFactoringContext _context;

    public CreateAccountCommandHandler(IFactoringContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            CompanyName = request.CompanyName,
            VATNumber = request.VATNumber,
            RegistrationNumber = request.RegistrationNumber,
            WithdrawAddress = request.WithdrawAddress,
            BusinessActivityId = request.BusinessActivityId,
            LegalFormId = request.LegalFormId,
            Administrators = new List<Administrator> { new Administrator { Address = request.Caller, CreatedAt = DateTime.Now.ToUniversalTime() } }
        };

        _context.Accounts.Add(account);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
