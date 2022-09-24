﻿using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Marketing.Infrastructure.Persistence.Interceptors;
using YourBrand.Marketing.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public class MarketingContext : DbContext, IMarketingContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public MarketingContext(
        DbContextOptions<MarketingContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("MarketingIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketingContext).Assembly);
    }

    public DbSet<Contact> Contacts { get; set; } = null!;

    public DbSet<Campaign> Campaigns { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; }  = null!;

    public DbSet<Discount> Discounts { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}