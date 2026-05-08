namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.SaveProduct;

public sealed class SaveProductCommandValidator : AbstractValidator<SaveProductCommand>
{
    public SaveProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");
    }
}
