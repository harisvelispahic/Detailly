namespace Detailly.Application.Modules.Sales.Orders.Commands.Submit;

public sealed class SubmitOrderCommandValidator : AbstractValidator<SubmitOrderCommand>
{
    public SubmitOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than zero.");
    }
}
