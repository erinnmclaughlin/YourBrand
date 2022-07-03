﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record UpdatePersonDetailsCommand(string PersonId, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<PersonDto>
{
    public class UpdatePersonDetailsCommandHandler : IRequestHandler<UpdatePersonDetailsCommand, PersonDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentPersonService _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public UpdatePersonDetailsCommandHandler(IApplicationDbContext context, ICurrentPersonService currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<PersonDto> Handle(UpdatePersonDetailsCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Include(u => u.Roles)
                .Include(u => u.Department)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            person.FirstName = request.FirstName;
            person.LastName = request.LastName;
            person.DisplayName = request.DisplayName;
            person.SSN = request.Ssn;
            person.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new PersonUpdated(person.Id, _currentPersonService.PersonId));

            return person.ToDto();
        }
    }
}