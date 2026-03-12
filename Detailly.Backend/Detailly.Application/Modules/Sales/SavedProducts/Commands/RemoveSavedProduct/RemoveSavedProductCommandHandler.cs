namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;

public sealed class RemoveSavedProductCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<RemoveSavedProductCommand>
{
    public async Task Handle(RemoveSavedProductCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var savedProduct = await context.SavedProducts
            .FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == request.ProductId, ct);

        if (savedProduct is null)
            return; // idempotent remove

        context.SavedProducts.Remove(savedProduct);
        await context.SaveChangesAsync(ct);
    }
}