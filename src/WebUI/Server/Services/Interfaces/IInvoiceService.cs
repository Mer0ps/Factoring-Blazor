using Application.Invoices.Queries;
using Domain.Entities;
using Domain.Models;
using Event = Mx.Notifier.Consumer.Models.Event;

namespace Mx.Blazor.DApp.Server.Services.Interfaces;

public interface IInvoiceService
{
    Task<EventNotification> Create(Event scEvent);
    Task<EventNotification> Confirm(Event scEvent);
    Task<EventNotification> AddInvoiceHistory(Event scEvent, Status status);
    Task<EventNotification> FullyFundInvoice(Event scEvent);
    Task<IEnumerable<InvoiceDto>> GetAll(long? accountId);
}
