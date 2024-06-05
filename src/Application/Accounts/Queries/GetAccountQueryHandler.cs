using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Queries;
public record GetAccountQuery : IRequest<AccountDto?>
{
    public string Address { get; init; }
}

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto?>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetAccountQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccountDto?> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _context.Accounts
            .Where(x => x.Administrators.Select(x => x.Address).Contains(request.Address))
            .ProjectTo<AccountDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (accounts.Count > 1)
        {
            throw new Exception("Multiple account found");
        }

        return accounts.FirstOrDefault();
    }
}
