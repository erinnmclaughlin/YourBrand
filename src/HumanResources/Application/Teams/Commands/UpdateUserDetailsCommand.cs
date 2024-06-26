﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record UpdateTeamCommand(string TeamId, string Name, string Description) : IRequest<TeamDto>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new PersonNotFoundException(request.TeamId);
            }

            team.UpdateName(request.Name);
            team.UpdateDescription(request.Description);

            await context.SaveChangesAsync(cancellationToken);

            return team.ToDto();
        }
    }
}