﻿namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
}