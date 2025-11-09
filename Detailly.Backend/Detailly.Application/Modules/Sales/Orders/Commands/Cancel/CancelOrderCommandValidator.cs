
namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;
public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Order ID must be greater than zero.");
    }
}
