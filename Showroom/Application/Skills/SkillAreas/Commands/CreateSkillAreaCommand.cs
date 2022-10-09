﻿using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.Skills.SkillAreas.Commands;

public record CreateSkillAreaCommand(string Name, int? IndustryId) : IRequest
{
    public class CreateSkillAreaCommandHandler : IRequestHandler<CreateSkillAreaCommand>
    {
        private readonly IShowroomContext context;

        public CreateSkillAreaCommandHandler(IShowroomContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateSkillAreaCommand request, CancellationToken cancellationToken)
        {
            var skillArea = await context.SkillAreas.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (skillArea is not null) throw new Exception();

            skillArea = new Domain.Entities.SkillArea
            {
                Id = Guid.NewGuid().ToString(),
                Slug = "",
                Name = request.Name,
                Industry = await context.Industries.FirstAsync(x => x.Id == request.IndustryId, cancellationToken)
            };

            context.SkillAreas.Add(skillArea);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
