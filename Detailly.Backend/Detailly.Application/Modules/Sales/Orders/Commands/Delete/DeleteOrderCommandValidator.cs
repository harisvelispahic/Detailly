
namespace Detailly.Application.Modules.Sales.Orders.Commands.Delete;
public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Order ID must be greater than zero.");
    }
}

