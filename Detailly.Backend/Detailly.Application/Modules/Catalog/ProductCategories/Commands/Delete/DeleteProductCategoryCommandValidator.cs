namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Delete;

public sealed class DeleteProductCategoryCommandValidator : AbstractValidator<DeleteProductCategoryCommand>
{
    public DeleteProductCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product category ID must be greater than zero.");
    }
}
