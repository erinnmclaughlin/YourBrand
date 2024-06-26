﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Discounts.Commands;

public record DeleteDiscountCommand(string Id) : IRequest
{
    public class DeleteDiscountCommandHandler(IMarketingContext context) : IRequestHandler<DeleteDiscountCommand>
    {
        public async Task Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await context.Discounts
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (discount is null) throw new Exception();

            context.Discounts.Remove(discount);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}