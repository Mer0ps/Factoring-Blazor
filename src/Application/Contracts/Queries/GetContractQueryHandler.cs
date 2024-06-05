using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Queries;
public record GetContractQuery : IRequest<ContractDto?>
{
    public long Id { get; init; }
}

public class GetContractQueryHandler : IRequestHandler<GetContractQuery, ContractDto?>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetContractQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ContractDto?> Handle(GetContractQuery request, CancellationToken cancellationToken)
    {
        return await _context.Contracts
            .Where(c => c.Id == request.Id)
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}