using AutoMapper;
using Domain.Entities;

namespace Application.LegalForms.Queries;
public class LegalFormDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<LegalForm, LegalFormDto>();
        }
    }
}
