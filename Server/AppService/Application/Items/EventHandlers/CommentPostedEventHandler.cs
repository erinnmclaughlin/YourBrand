﻿using Skynet.Application.Common.Interfaces;
using Skynet.Application.Common.Models;
using Skynet.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Skynet.Application.Items.EventHandlers;

public class CommentPostedEventHandler : INotificationHandler<DomainEventNotification<CommentPostedEvent>>
{
    private readonly ICatalogContext context;

    public CommentPostedEventHandler(ICatalogContext context)
    {
        this.context = context;
    }

    public async Task Handle(DomainEventNotification<CommentPostedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var item = await context.Items
            .FirstOrDefaultAsync(i => i.Id == domainEvent.ItemId, cancellationToken);

        if (item is null) return;

        item.CommentCount = await context.Comments.CountAsync(c => c.Item.Id == domainEvent.ItemId);

        await context.SaveChangesAsync(cancellationToken);
    }
}