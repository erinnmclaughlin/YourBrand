using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record GetProductVariants(string ProductId) : IRequest<IEnumerable<ProductVariantDto>>
{
    public class Handler : IRequestHandler<GetProductVariants, IEnumerable<ProductVariantDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariantDto>> Handle(GetProductVariants request, CancellationToken cancellationToken)
        {
            var variants = await _context.ProductVariants
            .AsSplitQuery()
            .AsNoTracking()
            .Include(pv => pv.Product)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Attribute)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Value)
            .Where(pv => pv.Product.Id == request.ProductId)
            .ToArrayAsync();

            return variants.Select(x => new ProductVariantDto(x.Id, x.Name, x.Description, x.SKU, GetImageUrl(x.Image), x.Price,
                x.Values.Select(x => new ProductVariantDtoOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name))));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}