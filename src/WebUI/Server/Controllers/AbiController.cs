using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Helpers;
using System.Net;
using System.Text;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AbiController : ControllerBase
{
    public AbiController()
    {
    }

    [HttpGet("get")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> Get(string name)
    {
        try
        {
            return Ok(Encoding.UTF8.GetString(ResourcesExtension.GetResourceByName(name)));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new HttpResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"An error occured while processing the request",
                Error = "Server error"
            });
        }
    }
}
