using AutoMapper;
using Domain.Entities;

namespace Application.ScAmins.Queries;
public class WhitelistedTokenDto
{
    public string TokenIdentifier { get; set; }
    public string? MoneyMarketAddress { get; set; }
    public string? HTokenIdentifier { get; set; }
    public DateTime CreatedAt { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<WhitelistedToken, WhitelistedTokenDto>();
        }
    }
}
