using YourBrand.Invoices.Application.Common.Models;
using YourBrand.Invoices.Contracts;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;
using YourBrand.Invoices.Domain.Events;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Client;

namespace YourBrand.Invoices.Application.Events;

public class InvoiceStatusChangedHandler : INotificationHandler<DomainEventNotification<InvoiceStatusChanged>>
{
    private readonly IInvoicesContext _context;
    private readonly IPaymentsClient _paymentsClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public InvoiceStatusChangedHandler(IInvoicesContext context, IPaymentsClient paymentsClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _paymentsClient = paymentsClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<InvoiceStatusChanged> notification, CancellationToken cancellationToken)
    {
        if (notification.DomainEvent.Status == InvoiceStatus.Paid)
        {
            await _publishEndpoint.Publish(new InvoicePaid(notification.DomainEvent.InvoiceId));
            return;
        }

        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.InvoiceId);

        if (invoice is not null)
        {
            if (invoice.Status == InvoiceStatus.Sent)
            {
                await _publishEndpoint.Publish(new InvoicesBatch(new[]
                {
                    new Contracts.Invoice(invoice.Id)
                }));

                var dueDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.AddDays(30), TimeZoneInfo.Local);

                invoice.UpdateTotals();

                await _paymentsClient.CreatePaymentAsync(new CreatePayment()
                {
                    InvoiceId = invoice.Id,
                    Currency = "SEK",
                    Amount = invoice.Total,
                    PaymentMethod = PaymentMethod.PlusGiro,
                    DueDate = dueDate,
                    Reference = Guid.NewGuid().ToUrlFriendlyString(),
                    Message = $"Betala faktura #{invoice.Id}",
                });
            }
        }
    }
}