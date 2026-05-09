namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("USER_NOT_AUTHENTICATED", "User is not authenticated.");

        var category = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (category is null)
            throw new DetaillyNotFoundException("Product category was not found.");

        var hasActiveProducts = await context.Products
            .AnyAsync(p => p.CategoryId == request.Id, ct);

        if (hasActiveProducts)
            throw new DetaillyBusinessRuleException(
                "CATEGORY_HAS_PRODUCTS",
                "Cannot delete a category that has active products assigned to it.");

        category.IsDeleted = true;
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
