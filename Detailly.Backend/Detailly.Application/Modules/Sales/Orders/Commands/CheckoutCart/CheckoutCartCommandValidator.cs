namespace Detailly.Application.Modules.Sales.Orders.Commands.CheckoutCart;

public sealed class CheckoutCartCommandValidator : AbstractValidator<CheckoutCartCommand>
{
    public CheckoutCartCommandValidator()
    {
        RuleFor(x => x.ShipToAddressId)
            .GreaterThan(0)
            .WithMessage("Shipping address ID must be greater than zero.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
