﻿
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;

namespace YourBrand.HumanResources.Application.Teams.Queries;

public record GetTeamMembershipsQuery(string TeamId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, HumanResources.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamMembershipDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetTeamMembershipsQuery, ItemsResult<TeamMembershipDto>>
    {
        public async Task<ItemsResult<TeamMembershipDto>> Handle(GetTeamMembershipsQuery request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                  .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new Exception("Team not found");
            }

            var query = context.TeamMemberships
                .Where(x => x.TeamId == request.TeamId)
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Person.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || p.Person.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || ((p.Person.DisplayName ?? "").ToLower().Contains(request.SearchString.ToLower()))
                    || p.Person.SSN.ToLower().Contains(request.SearchString.ToLower())
                    || p.Person.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var teamMemberships = await query
                .Include(x => x.Team)
                .Include(x => x.Person)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Person)
                .ThenInclude(x => x.Roles)
                .ToListAsync(cancellationToken);

            return new ItemsResult<TeamMembershipDto>(
                teamMemberships.Select(m => m.ToDto()),
                totalItems);
        }
    }
}