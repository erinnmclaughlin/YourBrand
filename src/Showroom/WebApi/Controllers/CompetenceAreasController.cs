﻿
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.CompetenceAreas;
using YourBrand.Showroom.Application.CompetenceAreas.Commands;
using YourBrand.Showroom.Application.CompetenceAreas.Queries;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class CompetenceAreasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<Results<CompetenceAreaDto>> GetCompetenceAreas(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetCompetenceAreasQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CompetenceAreaDto?> GetCompetenceArea(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCompetenceAreaQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<CompetenceAreaDto> CreateCompetenceArea(CreateCompetenceAreaDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateCompetenceAreaCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateCompetenceArea(string id, UpdateCompetenceAreaDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateCompetenceAreaCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCompetenceArea(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCompetenceAreaCommand(id), cancellationToken);
    }
}

public record CreateCompetenceAreaDto(string Name);

public record UpdateCompetenceAreaDto(string Name);