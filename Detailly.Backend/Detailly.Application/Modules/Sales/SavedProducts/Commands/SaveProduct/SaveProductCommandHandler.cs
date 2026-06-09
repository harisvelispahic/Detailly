using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.SaveProduct;

public sealed class SaveProductCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<SaveProductCommand>
{
    public async Task Handle(SaveProductCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
            throw new DetaillyNotFoundException("Product was not found.");

        if (!product.IsEnabled)
            throw new DetaillyBusinessRuleException("DISABLED_PRODUCT", "Disabled products cannot be saved.");

        // Include soft-deleted rows too, so we can revive an old saved record
        var existingSavedProduct = await context.SavedProducts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.ApplicationUserId == userId && x.ProductId == request.ProductId,
                ct);

        if (existingSavedProduct is not null)
        {
            // Already active -> ignore duplicate save
            if (!existingSavedProduct.IsDeleted)
                return;

            // Revive old soft-deleted row
            existingSavedProduct.IsDeleted = false;
            existingSavedProduct.ModifiedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return;
        }

        var savedProduct = new SavedProductEntity
        {
            ApplicationUserId = userId,
            ProductId = request.ProductId
        };

        context.SavedProducts.Add(savedProduct);
        await context.SaveChangesAsync(ct);
    }
}