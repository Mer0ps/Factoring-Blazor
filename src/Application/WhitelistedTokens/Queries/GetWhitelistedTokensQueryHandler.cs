using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Queries;
public record GetWhitelistedTokensQuery : IRequest<IEnumerable<WhitelistedTokenDto>>
{
}

public class GetWhitelistedTokensQueryHandler : IRequestHandler<GetWhitelistedTokensQuery, IEnumerable<WhitelistedTokenDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetWhitelistedTokensQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WhitelistedTokenDto>> Handle(GetWhitelistedTokensQuery request, CancellationToken cancellationToken)
    {
        return await _context.WhitelistedTokens
            .ProjectTo<WhitelistedTokenDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
