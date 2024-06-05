using AutoMapper;
using Domain.Entities;

namespace Application.BusinessActivities.Queries;
public class BusinessActivityDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public override string ToString()
    {
        return $"{Code} {Description}";
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BusinessActivity, BusinessActivityDto>();
        }
    }
}
