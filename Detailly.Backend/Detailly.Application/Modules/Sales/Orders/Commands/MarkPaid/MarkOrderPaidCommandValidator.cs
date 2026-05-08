namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkPaid;

public sealed class MarkOrderPaidCommandValidator : AbstractValidator<MarkOrderPaidCommand>
{
    public MarkOrderPaidCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than zero.");

        RuleFor(x => x.Provider)
            .MaximumLength(100)
            .When(x => x.Provider is not null)
            .WithMessage("Provider cannot exceed 100 characters.");

        RuleFor(x => x.ProviderTransactionId)
            .MaximumLength(200)
            .When(x => x.ProviderTransactionId is not null)
            .WithMessage("Provider transaction ID cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}
