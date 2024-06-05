using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Server.Services;
using System.Net;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConnectionController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(AccessToken accessToken)
        {
            var response = await _connectionService.Verify(accessToken.Value);
            if (response == null)
                return BadRequest(new HttpResponse()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Access token could no be generated",
                    Error = "Token error"
                });

            return Ok(response);
        }
    }
}
