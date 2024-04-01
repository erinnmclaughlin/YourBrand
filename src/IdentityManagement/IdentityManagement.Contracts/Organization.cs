﻿using System;

namespace YourBrand.IdentityManagement.Contracts;

public record CreateOrganization 
{
    public string Name{ get; init; }
    public string? FriendlyName { get; init; }
}

public record GetOrganization(string OrganizationId, string RequestedById);

public record GetOrganizationResponse(string Id, string TenantId, string Name, string? FriendlyName);

public record CreateOrganizationResponse(string Id, string Name, string? FriendlyName);

public record OrganizationCreated(string OrganizationId, string TenantId, string Name, string CreatedById);

public record OrganizationUpdated(string OrganizationId, string Name, string UpdatedById);

public record OrganizationDeleted(string OrganizationId, string DeletedById);

public record AddOrganizationUser(string OrganizationId, string UserId);

public record OrganizationUserAdded(string TenantId, string OrganizationId, string UserId);

public record RemoveOrganizationUser(string OrganizationId, string UserId);

public record OrganizationUserRemoved(string OrganizationId, string UserId);
