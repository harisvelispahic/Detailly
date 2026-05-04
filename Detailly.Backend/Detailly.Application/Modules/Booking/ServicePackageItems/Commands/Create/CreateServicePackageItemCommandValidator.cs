namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Create;

public sealed class CreateServicePackageItemCommandValidator : AbstractValidator<CreateServicePackageItemCommand>
{
    public CreateServicePackageItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.RequiredEmployees)
            .GreaterThan(0);
    }
}
