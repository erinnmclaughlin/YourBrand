﻿using YourBrand.Customers.Application.Queries;
using YourBrand.Customers.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Application.Queries;

public record GetPerson(string PersonId) : IRequest<PersonDto?>
{
    public class Handler : IRequestHandler<GetPerson, PersonDto?>
    {
        private readonly ICustomersContext _context;

        public Handler(ICustomersContext context)
        {
            _context = context;
        }

        public async Task<PersonDto?> Handle(GetPerson request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            return person is null
                ? null
                : person.ToDto();
        }
    }
}