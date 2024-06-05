using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Queries;
public record GetContractsForSupplierQuery : IRequest<IEnumerable<ContractDto>>
{
    public long? SupplierId { get; init; }
    public long? SupplierOnChainId { get; init; }
    public bool OnlySigned { get; init; }
}

public class GetContractsForSupplierQueryHandler : IRequestHandler<GetContractsForSupplierQuery, IEnumerable<ContractDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetContractsForSupplierQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContractDto>> Handle(GetContractsForSupplierQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Contracts.AsQueryable();

        if (request.OnlySigned)
        {
            query = query.Where(x => x.IsSigned);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(c => c.AccountSupplierId == request.SupplierId);
        }

        if (request.SupplierOnChainId.HasValue)
        {
            query = query.Where(c => c.AccountSupplier.OnChainId == request.SupplierOnChainId);
        }

        return await query
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}