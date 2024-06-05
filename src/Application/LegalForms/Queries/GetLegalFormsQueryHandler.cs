using Application.Common.Interfaces;
using Application.LegalForms.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ScAmins.Queries;
public record GetLegalFormsQuery : IRequest<IEnumerable<LegalFormDto>>
{
}

public class GetLegalFormsQueryHandler : IRequestHandler<GetLegalFormsQuery, IEnumerable<LegalFormDto>>
{
    private readonly IFactoringContext _context;
    private readonly IMapper _mapper;

    public GetLegalFormsQueryHandler(IFactoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LegalFormDto>> Handle(GetLegalFormsQuery request, CancellationToken cancellationToken)
    {
        return await _context.LegalForms
            .ProjectTo<LegalFormDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
