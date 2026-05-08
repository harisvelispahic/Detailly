namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkDelivered;

public sealed class MarkOrderDeliveredCommandValidator : AbstractValidator<MarkOrderDeliveredCommand>
{
    public MarkOrderDeliveredCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than zero.");

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .When(x => x.Note is not null)
            .WithMessage("Note cannot exceed 500 characters.");
    }
}
