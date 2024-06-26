﻿using YourBrand.IdentityManagement.Application.Tenants;

namespace YourBrand.IdentityManagement.Application.Users;

public record class UserDto(string Id, TenantDto Tenant, string FirstName, string LastName, string? DisplayName, string Email, DateTime Created, DateTime? LastModified);

public record class User2Dto(string Id, TenantDto Tenant, string FirstName, string LastName, string? DisplayName);