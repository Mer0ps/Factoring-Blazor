using AutoMapper;
using Domain.Entities;

namespace Application.ScAmins.Queries;
public class ScAdminDto
{
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ScAdmin, ScAdminDto>();
        }
    }
}
