using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record GetProductsByIds(long[] ProductIds, string? StoreId = null, string? BrandIdOrHandle = null) : IRequest<IEnumerable<ProductDto>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProductsByIds, IEnumerable<ProductDto>>
    {
        public async Task<IEnumerable<ProductDto>> Handle(GetProductsByIds request, CancellationToken cancellationToken)
        {
            var productIds = request.ProductIds.Distinct();

            var query = catalogContext.Products
                        .Where(x => productIds.Any(id => id == x.Id))
                        .IncludeAll()
                        .AsSplitQuery()
                        .AsNoTracking()
                        .AsQueryable()
                        .TagWith(nameof(GetProductsByIds));

            if (request.StoreId is not null)
            {
                query = query.Where(x => x.StoreId == request.StoreId);
            }

            if (request.BrandIdOrHandle is not null)
            {
                bool isBrandId = long.TryParse(request.BrandIdOrHandle, out var brandId);

                query = isBrandId ?
                    query.Where(pv => pv.BrandId == brandId)
                    : query.Where(pv => pv.Brand!.Handle == request.BrandIdOrHandle);
            }

            var products = await query
                .ToListAsync(cancellationToken);

            return products.Select(x => x.ToDto());
        }
    }
}