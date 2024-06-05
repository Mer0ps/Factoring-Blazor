using Application.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Controllers;
using System.Net;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContractController : ControllerApi
{
    public ContractController()
    {
    }


    [HttpGet("GetContractsClient")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContractDto>))]
    public async Task<IActionResult> GetContractsClient(long id)
    {
        try
        {
            var contracts = await Mediator.Send(new GetContractsForClientQuery { ClientId = id });
            return Ok(contracts);
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

    [HttpGet("GetContractsSupplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContractDto>))]
    public async Task<IActionResult> GetContractsSupplier(long id)
    {
        try
        {
            var contracts = await Mediator.Send(new GetContractsForSupplierQuery { SupplierId = id });
            return Ok(contracts);
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

    [HttpGet("GetContractsForSupplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ContractDto>))]
    public async Task<IActionResult> GetContractsForSupplier(long idSupplier)
    {
        try
        {
            var contracts = await Mediator.Send(new GetContractsForSupplierQuery { SupplierOnChainId = idSupplier, OnlySigned = true });
            return Ok(contracts);
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
