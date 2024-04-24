﻿using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Organizations;
using YourBrand.TimeReport.Application.Organizations.Commands;
using YourBrand.TimeReport.Application.Organizations.Queries;

namespace YourBrand.TimeReport.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<OrganizationDto>> GetOrganizations(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetOrganizationsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<OrganizationDto?> GetOrganization(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetOrganizationQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<OrganizationDto> CreateOrganization(CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateOrganizationCommand(Guid.NewGuid().ToString(), dto.Name, dto.ParentOrganizationId), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<OrganizationDto> UpdateOrganization(string id, UpdateOrganizationDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateOrganizationCommand(id, dto.Name), cancellationToken);
    }


    [HttpDelete("{id}")]
    public async Task DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrganizationCommand(id), cancellationToken);
    }
}

public record CreateOrganizationDto(string Name, string? ParentOrganizationId);

public record UpdateOrganizationDto(string Name);