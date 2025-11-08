
namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Shared;

public class CreateProductCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var normalized = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description?.Trim()))
            throw new ValidationException("Description is required.");

        if (request.Price == null || request.Price <= 0)
            throw new ValidationException("Price is required.");

        if (request.CategoryId == null || request.CategoryId <= 0)
            throw new ValidationException("Category is required.");

        // Check if a category with the same name already exists.
        bool exists = await context.Products
            .AnyAsync(x => x.Name == normalized, ct);

        if (exists)
        {
            throw new DetaillyConflictException("Name already exists.");
        }


        var product = new ProductEntity
        {
            Name = request.Name!.Trim(),
            Description = request.Description!.Trim(),
            Price = request.Price,
            ProductNumber = Guid.NewGuid().ToString(),
            CategoryId = request.CategoryId,
            IsEnabled = true, // deault IsEnabled
            Currency = request.Currency ?? CurrencyName.BAM,
            Tags = request.Tags,
            Inventory = new InventoryEntity
            {
                QuantityInStock = request.Inventory.QuantityInStock,
                ReorderLevel = request.Inventory.ReorderLevel ?? 0,
                ReorderQuantity = request.Inventory.ReorderQuantity ?? 0,
                IsDeleted = false,
                CreatedAtUtc = DateTime.UtcNow
            }

        };

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);

        product.Inventory.ProductId = product.Id;
        foreach (var item in request.Images)
        {
            var image = new ImageEntity
            {
                ImageUrl = item.ImageUrl,
                AltText = item.AltText,
                IsThumbnail = item.IsThumbnail ?? false,
                DisplayOrder = item.DisplayOrder ?? 0,
                ProductId = product.Id
            };
            await context.Images.AddAsync(image, ct);
        }

        await context.SaveChangesAsync(ct);

        return product.Id;

    }
}