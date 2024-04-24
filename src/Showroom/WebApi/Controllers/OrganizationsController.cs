﻿
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Organizations;
using YourBrand.Showroom.Application.Organizations.Commands;
using YourBrand.Showroom.Application.Organizations.Queries;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<Results<OrganizationDto>> GetOrganizations(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetOrganizationsQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<OrganizationDto?> GetOrganization(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetOrganizationQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateOrganization(CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateOrganizationCommand(dto.Id, dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateOrganization(string id, UpdateOrganizationDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateOrganizationCommand(id, dto.Name), cancellationToken);
    }


    [HttpDelete("{id}")]
    public async Task DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrganizationCommand(id), cancellationToken);
    }
}

public record CreateOrganizationDto(string Id, string Name);

public record UpdateOrganizationDto(string Name);