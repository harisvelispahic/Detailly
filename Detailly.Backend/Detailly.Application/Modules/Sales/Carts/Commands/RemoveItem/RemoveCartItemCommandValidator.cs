namespace Detailly.Application.Modules.Sales.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .GreaterThan(0)
            .WithMessage("Cart item ID must be greater than zero.");
    }
}
