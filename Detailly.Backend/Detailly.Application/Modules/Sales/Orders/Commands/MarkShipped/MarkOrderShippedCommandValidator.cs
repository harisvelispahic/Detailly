namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkShipped;

public sealed class MarkOrderShippedCommandValidator : AbstractValidator<MarkOrderShippedCommand>
{
    public MarkOrderShippedCommandValidator()
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
