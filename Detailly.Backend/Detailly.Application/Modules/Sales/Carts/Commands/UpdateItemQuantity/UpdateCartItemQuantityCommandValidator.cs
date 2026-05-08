namespace Detailly.Application.Modules.Sales.Carts.Commands.UpdateItemQuantity;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .GreaterThan(0)
            .WithMessage("Cart item ID must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1.");
    }
}
