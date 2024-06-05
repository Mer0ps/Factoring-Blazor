using Application.Common.Interfaces;
using Application.Contracts.Queries;
using Application.InvoiceHistories.Commands;
using Application.Invoices.Commands;
using Application.Invoices.Queries;
using Domain.Constants;
using Domain.Models;
using MediatR;
using Mx.Blazor.DApp.Properties;
using Mx.Blazor.DApp.Server.Helpers;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Abi;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Helper;
using Mx.NET.SDK.Provider;
using System.Numerics;
using System.Text;
using Decoder = Infrastructure.Helpers.Decoder;
using Event = Mx.Notifier.Consumer.Models.Event;
using Status = Domain.Entities.Status;

namespace Mx.Blazor.DApp.Server.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IMediator _mediator;
    private readonly AbiDefinition _abi;
    private readonly TypeValue _status;
    private readonly IFactoringContext _context;
    private readonly ApiProvider _apiProvider;
    public InvoiceService(IMediator mediator, IFactoringContext factoringContext)
    {
        _abi = AbiDefinition.FromJson(Encoding.UTF8.GetString(Resources.factoringAbi));
        _status = _abi.GetEventDefinition("invoice_confirm_event").Input[2].Type;
        _mediator = mediator;
        _context = factoringContext;
        _apiProvider = new(new ApiNetworkConfiguration(ContractConstants.NETWORK));
    }

    public async Task<IEnumerable<InvoiceDto>> GetAll(long? accountId)
    {
        var invoices = await _mediator.Send(new GetInvoicesQuery { AccountId = accountId });
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

    public async Task<EventNotification> Create(Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idContractInvoice = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var hash = Converter.HexToString(Decoder.ExtractResultScArg<string>(TypeValue.BytesValue, data[2]));
        var amount = Decoder.ExtractResultScArg<BigInteger>(TypeValue.BigUintTypeValue, data[3]);
        var due_date = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[4]);
        var idInvoice = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[5]);
        var timestamp = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[6]);
        var identifier = Converter.HexToString(Decoder.ExtractResultScArg<string>(TypeValue.BytesValue, data[7]));
        var rate = Decoder.ExtractResultScArg<int>(TypeValue.U32TypeValue, data[8]);

        try
        {
            await _mediator.Send(new CreateInvoiceCommand
            {
                InvoiceId = idInvoice,
                ContractId = idContractInvoice,
                Hash = hash,
                Identifier = identifier,
                Amount = amount,
                DueDate = due_date.ToDateTime(),
                TxHash = scEvent.TxHash,
                TxExecuteAt = timestamp.ToDateTime(),
                EuriborRate = rate,
            });

            return await GenerateInvoiceNotification(idContractInvoice);
        }
        catch (Exception ex)
        {
            throw new Exception("Erreur lors de l'insertion dans les tables X et Y", ex);
        }
    }

    public async Task<EventNotification> Confirm(Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idContract = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var idInvoice = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[2]);
        var status = Decoder.ToEnum<Status>(_status, data[3]);
        var timestamp = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[4]);

        await _mediator.Send(new CreateInvoiceHistoryCommand
        {
            InvoiceId = idInvoice,
            ContractId = idContract,
            Status = status,
            TxHash = scEvent.TxHash,
            TxExecuteAt = timestamp.ToDateTime(),
        });

        return await GenerateInvoiceNotification(idContract);
    }

    public async Task<EventNotification> AddInvoiceHistory(Event scEvent, Status status)
    {
        var data = scEvent.Topics.ToArray();
        var idContract = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var idInvoice = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[2]);
        var timestamp = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[3]);

        await _mediator.Send(new CreateInvoiceHistoryCommand
        {
            InvoiceId = idInvoice,
            ContractId = idContract,
            Status = status,
            TxHash = scEvent.TxHash,
            TxExecuteAt = timestamp.ToDateTime(),
        });

        return await GenerateInvoiceNotification(idContract);
    }

    public async Task<EventNotification> FullyFundInvoice(Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idContract = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var idInvoice = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[2]);
        var timestamp = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[3]);
        var commissionFee = Decoder.ExtractResultScArg<BigInteger>(TypeValue.BigUintTypeValue, data[4]);
        var financingFee = Decoder.ExtractResultScArg<BigInteger>(TypeValue.BigUintTypeValue, data[5]);

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            await _mediator.Send(new CreateInvoiceHistoryCommand
            {
                InvoiceId = idInvoice,
                ContractId = idContract,
                Status = Status.FullyFunded,
                TxHash = scEvent.TxHash,
                TxExecuteAt = timestamp.ToDateTime(),
            });

            await _mediator.Send(new CreateCollectedFeeCommand
            {
                InvoiceId = idInvoice,
                ContractId = idContract,
                CommissionFee = commissionFee,
                FinancingFee = financingFee,
            });

            transaction.Commit();
        }
        catch (Exception)
        {
            // TODO: Handle failure
        }

        return await GenerateInvoiceNotification(idContract);
    }

    private async Task<EventNotification> GenerateInvoiceNotification(long idContractInvoice)
    {
        var contract = await _mediator.Send(new GetContractQuery { Id = idContractInvoice });

        if (contract != null)
        {
            return new EventNotification
            {
                GroupsName = new List<string> { GroupHelper.GetGroupName(contract.IdSupplier), GroupHelper.GetGroupName(contract.IdClient) },
                EventName = ContractConstants.EVENT_REFRESH_INVOICE,
                Type = NotificationType.Group
            };
        }

        return null;
    }


}
