
namespace Detailly.Application.Modules.Sales.Orders.Commands.Pay;
public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Order ID must be greater than zero.");
    }
}
