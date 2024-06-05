using Application.ScAmins.Commands;
using Application.ScAmins.Queries;
using Application.WhitelistedTokens.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Controllers;
using Mx.Blazor.DApp.Services.Interfaces;
using System.Net;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AdministratorController : ControllerApi
{
    private readonly ITokenService _tokenService;

    public AdministratorController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpGet("getAllAdmins")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ScAdminDto>))]
    public async Task<IActionResult> GetAllAdmins()
    {
        try
        {
            var admins = await Mediator.Send(new GetScAdminsQuery());

            return Ok(admins);
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

    [HttpGet("GetAllWhitelistedTokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WhitelistedTokenDto>))]
    public async Task<IActionResult> GetAllWhitelistedTokens()
    {
        try
        {
            var tokens = await Mediator.Send(new GetWhitelistedTokensQuery());

            return Ok(tokens);
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

    [HttpGet("GetScTokens")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TokenInContractDto>))]
    public async Task<IActionResult> GetScTokens()
    {
        try
        {
            var tokens = await _tokenService.GetTokenInContract();

            return Ok(tokens);
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

    [HttpPut("updateToken")]
    public async Task<IActionResult> UpdateToken(WhitelistedTokenDto whitelistedTokenDto)
    {
        try
        {

            await Mediator.Send(new UpdateWhitelistedTokenCommand
            {
                TokenIdentifier = whitelistedTokenDto.TokenIdentifier,
                MoneyMarketAddress = whitelistedTokenDto.MoneyMarketAddress,
                HTokenIdentifier = whitelistedTokenDto.HTokenIdentifier,
            });

            return Ok();
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
