
namespace Detailly.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext ctx)
            : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await ctx.Products
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        if (product is null)
            throw new DetaillyNotFoundException($"Product (ID={request.Id}) was not found.");
        if (product.IsDeleted)
            throw new DetaillyNotFoundException("Cannot update a deleted product.");

        //product details 
        if (request.Name != null)
            product.Name = request.Name.Trim();

        if (request.Description != null)
            product.Description = request.Description.Trim();

        if (request.Price.HasValue)
            product.Price = request.Price.Value;

        if (request.Tags != null)
            product.Tags = request.Tags.Trim();

        //inventoy details
        if (request.Inventory.QuantityInStock.HasValue)
            product.Inventory.QuantityInStock = request.Inventory.QuantityInStock.Value;

        if (request.Inventory.ReorderLevel.HasValue)
            product.Inventory.ReorderLevel = request.Inventory.ReorderLevel.Value;

        if (request.Inventory.ReorderQuantity.HasValue)
            product.Inventory.ReorderQuantity = request.Inventory.ReorderQuantity.Value;

        ////images details
        //if (request.Images is not null)
        //{
        //    foreach (var image in request.Images)
        //    {

        //    }
        //}

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
