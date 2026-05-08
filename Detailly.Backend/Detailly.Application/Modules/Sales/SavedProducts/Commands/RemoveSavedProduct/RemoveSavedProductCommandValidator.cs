namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;

public sealed class RemoveSavedProductCommandValidator : AbstractValidator<RemoveSavedProductCommand>
{
    public RemoveSavedProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");
    }
}
