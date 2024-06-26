﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences.Queries;

public record GetExperienceQuery(string PersonProfileId, string Id) : IRequest<ExperienceDto>
{
    sealed class GetPersonProfileQueryHandler(
        IShowroomContext context,
        IUserContext userContext) : IRequestHandler<GetExperienceQuery, ExperienceDto?>
    {
        public async Task<ExperienceDto?> Handle(GetExperienceQuery request, CancellationToken cancellationToken)
        {
            var experience = await context
               .PersonProfileExperiences
                .Include(x => x.Employment)
                .ThenInclude(x => x.Employer)
               .Include(x => x.Company)
                .ThenInclude(x => x.Industry)
               .Include(x => x.Skills)
               .ThenInclude(x => x.PersonProfileSkill)
                .ThenInclude(x => x.Skill)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.Industry)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (experience is null)
            {
                return null;
            }

            return experience.ToDto();
        }
    }
}