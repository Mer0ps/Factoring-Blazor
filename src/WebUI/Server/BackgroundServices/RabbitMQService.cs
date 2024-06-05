using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Administrators.Commands;
using Application.Contracts.Commands;
using Application.ScAmins.Commands;
using Domain.Constants;
using Domain.Models;
using Infrastructure.Helpers;
using Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Mx.Blazor.DApp.Server.Helpers;
using Mx.Blazor.DApp.Server.Services;
using Mx.Blazor.DApp.Server.Services.Interfaces;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.Notifier.Consumer;
using Mx.Notifier.Consumer.Models;
using BlockEvent = Mx.Notifier.Consumer.Models.BlockEvent;
using Event = Mx.Notifier.Consumer.Models.Event;

namespace Mx.Blazor.DApp.Server.BackgroundServices;

/// <summary>
/// This consumer should move to EventNotifier.Consumer and implement InboxPattern to consume events
/// </summary>
public class RabbitMQService : BackgroundService
{
    private readonly ILogger<RabbitMQService> _logger;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly RabbitMqInitializer _rabbitMqInitializer;

    private readonly Dictionary<string, Func<IServiceProvider, Event, Task>> _eventHandlers;

    public RabbitMQService(ILogger<RabbitMQService> logger, IHubContext<NotificationHub> hubContext,
    IServiceScopeFactory serviceScopeFactory, IOptions<EventNotifierSettings> eventNotifierSettings)
    {
        _logger = logger;
        _hubContext = hubContext;
        _serviceScopeFactory = serviceScopeFactory;

        var config = new RabbitConfig
        {
            HostName = eventNotifierSettings.Value.HostName,
            QueueName = eventNotifierSettings.Value.QueueName,
            UserName = eventNotifierSettings.Value.UserName,
            Password = eventNotifierSettings.Value.Password,
            AutoAck = false,
        };

        _eventHandlers = new Dictionary<string, Func<IServiceProvider, Event, Task>>
            {
                { "invoice_add_event", HandleInvoiceAddEvent },
                { "invoice_confirm_event", HandleInvoiceConfirmEvent },
                { "invoice_fund_event", HandleInvoiceFundEvent },
                { "invoice_pay_event", HandleInvoicePayEvent },
                { "invoice_fully_fund_event", HandleInvoiceFullyFundEvent },
                { "contract_create_event", HandleContractCreateEvent },
                { "contract_sign_event", HandleContractSignEvent },
                { "company_create_event", HandleCompanyCreateEvent },
                { "company_new_score_event", HandleCompanyNewScoreEvent },
                { "company_add_admin_event", HandleCompanyAddAdminEvent },
                { "sc_add_admin_event", HandleAddScAdminEvent },
                { "sc_remove_admin_event", HandleRemoveScAdminEvent },
                { "sc_add_token_event", HandleAddWhitelistedTokenEvent },
                { "sc_remove_token_event", HandleRemoveWhitelistedTokenEvent },
            };

        _rabbitMqInitializer = new RabbitMqInitializer(config);
        _rabbitMqInitializer.MessageReceivedAsync += HandleCustomMessage;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        stoppingToken.ThrowIfCancellationRequested();

        _rabbitMqInitializer.Initialize();

        return Task.CompletedTask;
    }

    private async Task HandleCustomMessage(BlockEvent? blockEvent)
    {
        var filteredEvent = blockEvent?.Events?.Where(x => !string.IsNullOrEmpty(x.Name) && x.Address == ContractConstants.FACTORING_CONTRACT) ?? Enumerable.Empty<Event>();

        foreach (var scEvent in filteredEvent)
        {
            if (_eventHandlers.TryGetValue(scEvent.Name, out var handler))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    await handler(scope.ServiceProvider, scEvent);
                }
            }
        }
    }

    private async Task HandleInvoiceAddEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var eventNotification = await serviceProvider.GetRequiredService<IInvoiceService>().Create(scEvent);
        await SendNotification(eventNotification);
    }

    private async Task HandleInvoiceConfirmEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var eventNotification = await serviceProvider.GetRequiredService<IInvoiceService>().Confirm(scEvent);
        await SendNotification(eventNotification);
    }

    private async Task HandleInvoiceFundEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var eventNotification = await serviceProvider.GetRequiredService<IInvoiceService>().AddInvoiceHistory(scEvent, Domain.Entities.Status.PartiallyFunded);
        await SendNotification(eventNotification);
    }
    private async Task HandleInvoiceFullyFundEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var eventNotification = await serviceProvider.GetRequiredService<IInvoiceService>().FullyFundInvoice(scEvent);
        await SendNotification(eventNotification);
    }

    private async Task HandleInvoicePayEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var eventNotification = await serviceProvider.GetRequiredService<IInvoiceService>().AddInvoiceHistory(scEvent, Domain.Entities.Status.Payed);
        await SendNotification(eventNotification);
    }

    private async Task HandleContractCreateEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var data = scEvent.Topics.ToArray();

        var idSupplier = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var idClient = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[2]);
        var idContract = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[3]);

        var accountsDto = await serviceProvider.GetRequiredService<IMediator>().Send(new GetAccountsQuery()
        {
            AccountIds = new List<long> { idSupplier, idClient }
        });

        var accounts = accountsDto.ToDictionary(x => x.OnChainId.Value);

        var createContractCommand = new CreateContractCommand
        {
            ContractId = idContract,
            SupplierId = accounts[idSupplier].Id,
            ClientId = accounts[idClient].Id,
        };

        await serviceProvider.GetRequiredService<IMediator>().Send(createContractCommand);

        await _hubContext.Clients
            .Groups(new List<string> { GroupHelper.GetGroupName(createContractCommand.SupplierId), GroupHelper.GetGroupName(createContractCommand.ClientId) })
            .SendAsync(ContractConstants.EVENT_REFRESH_CONTRACT);
    }

    private async Task HandleContractSignEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idContract = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);

        var signResult = await serviceProvider.GetRequiredService<IMediator>().Send(new SignContractCommand
        {
            ContractId = idContract,
        });

        if (signResult != null)
        {
            await _hubContext.Clients
                .Groups(new List<string> { GroupHelper.GetGroupName(signResult.SupplierId), GroupHelper.GetGroupName(signResult.ClientId) })
                .SendAsync(ContractConstants.EVENT_REFRESH_CONTRACT);
        }
    }

    private async Task HandleCompanyCreateEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idAccountOff = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var idAccount = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[2]);
        var score = Decoder.ExtractResultScArg<int>(TypeValue.U8TypeValue, data[3]);
        var fee = Decoder.ExtractResultScArg<int>(TypeValue.U64TypeValue, data[4]);


        await serviceProvider.GetRequiredService<IMediator>().Send(new InitCompanyAccountCommand
        {
            AccountIdOffChain = idAccountOff,
            AccountId = idAccount,
            Score = score,
            Fee = fee,
        });
    }

    private async Task HandleCompanyNewScoreEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var idAccount = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var score = Decoder.ExtractResultScArg<int>(TypeValue.U8TypeValue, data[2]);

        await serviceProvider.GetRequiredService<IMediator>().Send(new UpdateAccountScoreCommand { AccountIdOffChain = idAccount, Score = score });
    }

    private async Task HandleCompanyAddAdminEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var data = scEvent.Topics.ToArray();
        var accountId = Decoder.ExtractResultScArg<long>(TypeValue.U64TypeValue, data[1]);
        var address = Decoder.ExtractResultScArg<Address>(TypeValue.AddressValue, data[2]);

        await serviceProvider.GetRequiredService<IMediator>().Send(new CreateAdministratorCommand { AccountId = accountId, Address = address.Bech32 });
    }

    private async Task HandleAddScAdminEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var address = Decoder.ExtractResultScArg<Address>(TypeValue.AddressValue, scEvent.Topics.ToArray()[1]);
        await serviceProvider.GetRequiredService<IMediator>().Send(new CreateScAdminCommand { Address = address.Bech32 });
        await _hubContext.Clients
                .All
                .SendAsync(ContractConstants.EVENT_REFRESH_ADMIN);
    }

    private async Task HandleRemoveScAdminEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var address = Decoder.ExtractResultScArg<Address>(TypeValue.AddressValue, scEvent.Topics.ToArray()[1]);
        await serviceProvider.GetRequiredService<IMediator>().Send(new DeleteScAdminCommand { Address = address.Bech32 });
        await _hubContext.Clients
                .All
                .SendAsync(ContractConstants.EVENT_REFRESH_ADMIN);
    }

    private async Task HandleAddWhitelistedTokenEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var tokenIdentifier = Decoder.ExtractResultScArg<string>(TypeValue.TokenIdentifierValue, scEvent.Topics.ToArray()[1]);
        await serviceProvider.GetRequiredService<IMediator>().Send(new CreateWhitelistedTokenCommand { TokenIdentifier = tokenIdentifier });
        await _hubContext.Clients
                .All
                .SendAsync(ContractConstants.EVENT_REFRESH_WHITELISTED_TOKEN);
    }

    private async Task HandleRemoveWhitelistedTokenEvent(IServiceProvider serviceProvider, Event scEvent)
    {
        var tokenIdentifier = Decoder.ExtractResultScArg<string>(TypeValue.TokenIdentifierValue, scEvent.Topics.ToArray()[1]);
        await serviceProvider.GetRequiredService<IMediator>().Send(new DeleteWhitelistedTokenCommand { TokenIdentifier = tokenIdentifier });
        await _hubContext.Clients
                .All
                .SendAsync(ContractConstants.EVENT_REFRESH_WHITELISTED_TOKEN);
    }


    private async Task SendNotification(EventNotification? eventNotification)
    {
        if (eventNotification != null)
        {
            switch (eventNotification.Type)
            {
                case NotificationType.All:
                    await _hubContext.Clients.All.SendAsync(eventNotification.EventName);
                    break;
                case NotificationType.Group:
                    await _hubContext.Clients.Groups(eventNotification.GroupsName.ToList()).SendAsync(eventNotification.EventName);
                    break;
                case NotificationType.Client:
                    break;
            }
        }
    }
}
