
namespace Detailly.Application.Modules.Sales.Orders.Commands.Ship;
public class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
{
    public ShipOrderCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Order ID must be greater than zero.");
    }
}
