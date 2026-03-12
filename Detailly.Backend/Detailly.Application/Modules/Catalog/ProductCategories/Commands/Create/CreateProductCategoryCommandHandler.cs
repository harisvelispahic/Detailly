namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Create;

public sealed class CreateProductCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCategoryCommand, int>
{
    public async Task<int> Handle(CreateProductCategoryCommand request, CancellationToken ct)
    {
        var category = new ProductCategoryEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsEnabled = true
        };

        context.ProductCategories.Add(category);
        await context.SaveChangesAsync(ct);

        return category.Id;
    }
}