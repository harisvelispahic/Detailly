namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;

public sealed class RemoveSavedProductCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<RemoveSavedProductCommand>
{
    public async Task Handle(RemoveSavedProductCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var savedProduct = await context.SavedProducts
            .FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, ct);

        if (savedProduct is null)
            return; // idempotent remove

        context.SavedProducts.Remove(savedProduct);
        await context.SaveChangesAsync(ct);
    }
}