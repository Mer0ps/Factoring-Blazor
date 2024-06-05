using Application.BusinessActivities.Queries;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Queries;
public record GetBusinessActivitiesQuery : IRequest<IEnumerable<BusinessActivityDto>>
{
}

public class GetBusinessActivitiesQueryHandler : IRequestHandler<GetBusinessActivitiesQuery, IEnumerable<BusinessActivityDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetBusinessActivitiesQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BusinessActivityDto>> Handle(GetBusinessActivitiesQuery request, CancellationToken cancellationToken)
    {
        return await _context.BusinessActivities
            .ProjectTo<BusinessActivityDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
