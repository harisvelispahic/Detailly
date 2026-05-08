namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommandValidator : AbstractValidator<DisableProductCommand>
{
    public DisableProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");
    }
}
