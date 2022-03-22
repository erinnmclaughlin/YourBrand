﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.CompetenceAreas;
using YourBrand.Showroom.Domain.Entities;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.Organizations.Queries;

public class GetOrganizationsQuery : IRequest<Results<OrganizationDto>>
{
    public GetOrganizationsQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }
    public int PageSize { get; }
    public string? SearchString { get; }
    public string? SortBy { get; }
    public Common.Models.SortDirection? SortDirection { get; }

    class GetOrganizationsQueryHandler : IRequestHandler<GetOrganizationsQuery, Results<OrganizationDto>>
    {
        private readonly IShowroomContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetOrganizationsQueryHandler(
            IShowroomContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Results<OrganizationDto>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Organization> result = _context
                    .Organizations
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Showroom.Application.SortDirection.Descending : Showroom.Application.SortDirection.Ascending);
            }

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new Results<OrganizationDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}
