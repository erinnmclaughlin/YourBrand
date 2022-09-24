﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Infrastructure.Persistence;
using YourBrand.Invoicing.Infrastructure.Persistence.Outbox;

namespace YourBrand.Invoicing.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly InvoicingContext dbContext;
    private readonly IDomainEventDispatcher domainEventDispatcher;
    private readonly ILogger<ProcessOutboxMessagesJob> logger;

    public ProcessOutboxMessagesJob(InvoicingContext dbContext, IDomainEventDispatcher domainEventDispatcher,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        this.dbContext = dbContext;
        this.domainEventDispatcher = domainEventDispatcher;
        this.logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Processing Outbox");

        List<OutboxMessage> messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            DomainEvent? domainEvent = JsonConvert
                .DeserializeObject<DomainEvent>(outboxMessage.Content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            if (domainEvent is null)
            {
                continue;
            }

            await domainEventDispatcher.Dispatch(domainEvent, context.CancellationToken);

            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        logger.LogDebug("Finished processing Outbox");

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}

