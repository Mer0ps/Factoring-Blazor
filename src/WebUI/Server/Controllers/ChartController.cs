using Application.Invoices.Queries;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Controllers;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using System.Net;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;


public class ChartController : ControllerApi
{
    private readonly IChartService _chartService;
    public ChartController(IChartService chartService)
    {
        _chartService = chartService;
    }

    [HttpGet("getTotalFinanced")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TotalFinancedDto>))]
    public async Task<IActionResult> GetTotalFinanced(long? accountId)
    {
        try
        {
            return Ok(await _chartService.GetTotalFinanced(accountId));
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

    [HttpGet("getTotalCollectedFee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TotalCollectedFeeDto>))]
    public async Task<IActionResult> GetTotalCollectedFee(long? accountId)
    {
        try
        {
            return Ok(await _chartService.GetTotalCollectedFee(accountId));
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

    [HttpGet("getOverdue")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OverdueDto>))]
    public async Task<IActionResult> GetOverdue()
    {
        try
        {
            return Ok(await Mediator.Send(new GetOverduedQuery()));
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
