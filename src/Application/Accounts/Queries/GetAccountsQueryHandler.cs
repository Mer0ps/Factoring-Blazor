using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Queries;
public record GetAccountsQuery : IRequest<IEnumerable<AccountDto>>
{
    public IEnumerable<long>? AccountIds { get; init; }
}

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, IEnumerable<AccountDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetAccountsQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = _context.Accounts.AsQueryable();

        if (request.AccountIds != null && request.AccountIds.Any())
        {
            accounts = accounts.Where(x => x.OnChainId.HasValue && request.AccountIds.Contains(x.OnChainId.Value));
        }

        return await accounts.ProjectTo<AccountDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
