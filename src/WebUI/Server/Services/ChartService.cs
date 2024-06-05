using Application.Invoices.Queries;
using Domain.Constants;
using MediatR;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Server.Services;

public class ChartService : IChartService
{
    private readonly IMediator _mediator;
    private readonly ApiProvider _apiProvider;
    public ChartService(IMediator mediator)
    {
        _mediator = mediator;
        _apiProvider = new(new ApiNetworkConfiguration(ContractConstants.NETWORK));
    }

    public async Task<IEnumerable<InvoiceDto>> GetAll()
    {
        var invoices = await _mediator.Send(new GetInvoicesQuery());
        var identifiers = invoices.Select(x => x.Identifier).Distinct().ToList();

        var esdtByIdentifier = new Dictionary<string, ESDT>();

        foreach (var identifier in identifiers)
        {
            var tokenDto = await _apiProvider.GetToken(identifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            esdtByIdentifier.Add(identifier, esdt);
        }


        foreach (var invoice in invoices)
        {
            invoice.Esdt = esdtByIdentifier[invoice.Identifier];
        }
        return invoices;
    }


    public async Task<IEnumerable<TotalFinancedDto>> GetTotalFinanced(long? accountId)
    {
        var datas = await _mediator.Send(new GetTotalFinancedQuery { AccountId = accountId });
        var identifiers = datas.Where(x => !string.IsNullOrEmpty(x.Identifier)).Select(x => x.Identifier).Distinct().ToList();

        var esdtByIdentifier = new Dictionary<string, ESDT>();

        foreach (var identifier in identifiers)
        {
            var tokenDto = await _apiProvider.GetToken(identifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            esdtByIdentifier.Add(identifier, esdt);
        }


        foreach (var month in datas.Where(x => !string.IsNullOrEmpty(x.Identifier)))
        {
            month.Esdt = esdtByIdentifier[month.Identifier];
        }

        return datas;
    }

    public async Task<IEnumerable<TotalCollectedFeeDto>> GetTotalCollectedFee(long? accountId)
    {
        var datas = await _mediator.Send(new GetTotalCollectedFeeQuery { AccountId = accountId });
        var identifiers = datas.Where(x => !string.IsNullOrEmpty(x.Identifier)).Select(x => x.Identifier).Distinct().ToList();

        var esdtByIdentifier = new Dictionary<string, ESDT>();

        foreach (var identifier in identifiers)
        {
            var tokenDto = await _apiProvider.GetToken(identifier);
            var esdt = ESDT.TOKEN(tokenDto.Name, tokenDto.Identifier, tokenDto.Decimals);
            esdtByIdentifier.Add(identifier, esdt);
        }


        foreach (var month in datas.Where(x => !string.IsNullOrEmpty(x.Identifier)))
        {
            month.Esdt = esdtByIdentifier[month.Identifier];
        }

        return datas;
    }
}
