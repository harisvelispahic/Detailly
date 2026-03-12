namespace Detailly.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await context.Products
            .Include(x => x.Inventory)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null || product.IsDeleted)
            throw new DetaillyNotFoundException($"Product (ID={request.Id}) was not found.");

        if (request.Name is not null)
            product.Name = request.Name.Trim();

        if (request.Description is not null)
            product.Description = request.Description.Trim();

        if (request.Price.HasValue)
            product.Price = request.Price.Value;

        if (request.Tags is not null)
            product.Tags = request.Tags.Trim();

        if (request.Inventory is not null)
        {
            product.Inventory.QuantityInStock = request.Inventory.QuantityInStock ?? product.Inventory.QuantityInStock;
            product.Inventory.ReorderLevel = request.Inventory.ReorderLevel ?? product.Inventory.ReorderLevel;
            product.Inventory.ReorderQuantity = request.Inventory.ReorderQuantity ?? product.Inventory.ReorderQuantity;
            product.Inventory.ModifiedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}