namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Update;

public sealed class UpdateServicePackageItemCommandValidator : AbstractValidator<UpdateServicePackageItemCommand>
{
    public UpdateServicePackageItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        When(x => x.Name is not null, () =>
            RuleFor(x => x.Name!).NotEmpty().MaximumLength(200));

        When(x => x.Description is not null, () =>
            RuleFor(x => x.Description!).MaximumLength(2000));

        When(x => x.Price.HasValue, () =>
            RuleFor(x => x.Price!.Value).GreaterThanOrEqualTo(0));

        When(x => x.DurationMinutes.HasValue, () =>
            RuleFor(x => x.DurationMinutes!.Value).GreaterThan(0));

        When(x => x.RequiredEmployees.HasValue, () =>
            RuleFor(x => x.RequiredEmployees!.Value).GreaterThan(0));
    }
}
