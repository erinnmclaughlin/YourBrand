﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using YourBrand.Identity;
using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Domain.Common;
using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Notifications.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public AuditableEntitySaveChangesInterceptor(
        ITenantService tenantService,
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _tenantService = tenantService;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IHasTenant>())
        {
            if (entry.State == EntityState.Added)
            {

                entry.Entity.TenantId = _tenantService.TenantId.GetValueOrDefault();
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = _currentUserService.UserId!;
                entry.Entity.Created = _dateTime.Now;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedById = _currentUserService.UserId;
                entry.Entity.LastModified = _dateTime.Now;
            }
            else if (entry.State == EntityState.Deleted)
            {
                if (entry.Entity is ISoftDelete softDelete)
                {
                    softDelete.DeletedById = _currentUserService.UserId;
                    softDelete.Deleted = _dateTime.Now;

                    entry.State = EntityState.Modified;
                }
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}