using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
namespace YourBrand.HumanResources.Application.Teams.Commands;

public record AddTeamMemberCommand(string TeamId, string PersonId) : IRequest
{
    public class Handler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<AddTeamMemberCommand>
    {
        public async Task Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .Include(x => x.Memberships)
                .FirstOrDefaultAsync(i => i.Id == request.TeamId, cancellationToken);

            if (team is null) throw new Exception();

            var user = await context.Persons
                .FirstOrDefaultAsync(i => i.Id == request.PersonId, cancellationToken);

            if (user is null) throw new Exception();

            team.AddMember(user);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new Contracts.TeamMemberAdded(team.Id, user.Id));

        }
    }
}