using Application.Accounts.Queries;
using Infrastructure.Identity;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Mx.NET.SDK.NativeAuthServer;
using Mx.NET.SDK.NativeAuthServer.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mx.Blazor.DApp.Server.Services
{
    public interface IConnectionService
    {
        Task<ConnectionToken?> Verify(string accessToken);
    }

    public class ConnectionService : IConnectionService
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public ConnectionService(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task<ConnectionToken?> Verify(string accessToken)
        {
            var nativeAuthServer = new NativeAuthServer(new NativeAuthServerConfig());
            try
            {
                var nativeAuthToken = nativeAuthServer.Validate(accessToken);

                var jwtToken = await GenerateJwtToken(nativeAuthToken.Address, accessToken, nativeAuthToken.Body.TTL);
                return new ConnectionToken(
                    new AccountToken()
                    {
                        Address = nativeAuthToken.Address,
                        Signature = nativeAuthToken.Signature
                    },
                    jwtToken);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<string> GenerateJwtToken(string address, string accessToken, int ttl)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWT:SecurityKey"));
            var account = await _mediator.Send(new GetAccountQuery { Address = address });

            var claims = new List<Claim>
            {
                new Claim("address", address),
                new Claim("accessToken", accessToken)
            };

            if (account != null && !string.IsNullOrEmpty(account.Id.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(ttl),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
