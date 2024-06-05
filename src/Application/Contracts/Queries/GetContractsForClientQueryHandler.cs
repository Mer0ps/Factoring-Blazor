using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Queries;
public record GetContractsForClientQuery : IRequest<IEnumerable<ContractDto>>
{
    public long ClientId { get; init; }
}

public class GetContractsForClientQueryHandler : IRequestHandler<GetContractsForClientQuery, IEnumerable<ContractDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetContractsForClientQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContractDto>> Handle(GetContractsForClientQuery request, CancellationToken cancellationToken)
    {
        return await _context.Contracts
            .Where(c => c.AccountClientId == request.ClientId)
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}