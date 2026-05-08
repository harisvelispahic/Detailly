namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommandValidator : AbstractValidator<EnableProductCommand>
{
    public EnableProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID must be greater than zero.");
    }
}
