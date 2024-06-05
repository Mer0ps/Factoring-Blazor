using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.BusinessActivities.Queries;
using Application.LegalForms.Queries;
using Application.ScAmins.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Controllers;
using System.Net;
using System.Security.Claims;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerApi
{

    public AccountController()
    {
    }

    [HttpGet("getAll")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountDto>))]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var accountsDto = await Mediator.Send(new GetAccountsQuery());

            return Ok(accountsDto);
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

    [HttpGet("getLegalForms")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LegalFormDto>))]
    public async Task<IActionResult> GetLegalForms()
    {
        try
        {
            var legalFormDtos = await Mediator.Send(new GetLegalFormsQuery());

            return Ok(legalFormDtos);
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

    [HttpGet("getBusinessActivities")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BusinessActivityDto>))]
    public async Task<IActionResult> GetBusinessActivities()
    {
        try
        {
            var businessActivityDtos = await Mediator.Send(new GetBusinessActivitiesQuery());

            return Ok(businessActivityDtos);
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

    [HttpGet("isDappAdministrator")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> IsDappAdministrator(string address)
    {
        try
        {
            var scAdminAddress = await Mediator.Send(new GetScAdminQuery { Address = address });

            return Ok(!string.IsNullOrEmpty(scAdminAddress));
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountDto))]
    public async Task<IActionResult> Get(string address)
    {
        try
        {
            var account = await Mediator.Send(new GetAccountQuery
            {
                Address = address,
            });

            if (account == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, new HttpResponse()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = $"This address is not linked to an account",
                    Error = "NotFound"
                });
            }

            return Ok(account);
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

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] AccountDto account)
    {
        try
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var caller = identity?.Claims.FirstOrDefault(i => i.Type == "address")?.Value;


            if (caller == null)
                return Unauthorized(new HttpResponse()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User identity could no be retrieved",
                    Error = "Unauthorized"
                });

            await Mediator.Send(new CreateAccountCommand
            {
                CompanyName = account.CompanyName,
                RegistrationNumber = account.RegistrationNumber,
                VATNumber = account.VATNumber,
                WithdrawAddress = account.WithdrawAddress,
                BusinessActivityId = account.BusinessActivityId.Value,
                LegalFormId = account.LegalFormId.Value,
                Caller = caller,
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
