using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Queries;
public record GetScAdminsQuery : IRequest<IEnumerable<ScAdminDto>>
{
}

public class GetScAdminsQueryHandler : IRequestHandler<GetScAdminsQuery, IEnumerable<ScAdminDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetScAdminsQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScAdminDto>> Handle(GetScAdminsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ScAdmins
            .ProjectTo<ScAdminDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
