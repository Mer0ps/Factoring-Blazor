using Application.Invoices.Queries;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mx.Blazor.DApp.Controllers;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Pinata.Client;
using System.Net;
using System.Security.Claims;
using HttpResponse = Domain.Models.HttpResponse;

namespace Mx.Blazor.DApp.Server.Controllers;


public class InvoiceController : ControllerApi
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPFSSettings _ipfsSettings;
    public InvoiceController(IInvoiceService invoiceService, IOptions<IPFSSettings> ipfsSettings)
    {
        _invoiceService = invoiceService;
        _ipfsSettings = ipfsSettings.Value;
    }

    [HttpGet("getAll")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<InvoiceDto>))]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var accountId = identity?.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
            long? accountIdParsed = string.IsNullOrEmpty(accountId) ? null : long.Parse(accountId);

            return Ok(await _invoiceService.GetAll(accountIdParsed));
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

    [HttpGet("{idContract}/{idInvoice}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InvoiceDetailDto))]
    public async Task<IActionResult> GetInvoice(long idContract, long idInvoice)
    {
        try
        {
            var invoiceWithHistory = await Mediator.Send(new GetInvoiceWithHistoryQuery { ContractId = idContract, InvoiceId = idInvoice });

            return Ok(invoiceWithHistory);
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

    [HttpPost("uploadToIpfs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> PostToIpfs([FromForm] IFormFile invoice)
    {
        try
        {
            var config = new Config
            {
                ApiKey = _ipfsSettings.ApiKey,
                ApiSecret = _ipfsSettings.ApiSecret
            };

            var client = new PinataClient(config);
            var hash = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                await invoice.CopyToAsync(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var fileContent = new StreamContent(memoryStream);

                var response = await client.Pinning.PinFileToIpfsAsync(content =>
                {
                    content.AddPinataFile(fileContent, invoice.FileName);
                });

                if (response.IsSuccess)
                {
                    hash = response.IpfsHash;
                }
            }

            return Ok(hash);
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
