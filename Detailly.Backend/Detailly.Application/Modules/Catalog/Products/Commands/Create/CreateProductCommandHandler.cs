using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public sealed class CreateProductCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new ProductEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            ProductNumber = Guid.NewGuid().ToString(),
            Price = request.Price,
            CategoryId = request.CategoryId,
            IsEnabled = true,
            Currency = request.Currency ?? CurrencyName.BAM,
            Tags = request.Tags?.Trim(),
            Inventory = new InventoryEntity
            {
                QuantityInStock = request.Inventory?.QuantityInStock ?? 0,
                ReorderLevel = request.Inventory?.ReorderLevel ?? 0,
                ReorderQuantity = request.Inventory?.ReorderQuantity ?? 0,
                IsDeleted = false,
                CreatedAtUtc = DateTime.UtcNow
            },
            Images = (request.Images ?? [])
                .Select(x => new ImageEntity
                {
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    IsThumbnail = x.IsThumbnail ?? false,
                    DisplayOrder = x.DisplayOrder ?? 0
                })
                .ToList()
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);

        return product.Id;
    }
}