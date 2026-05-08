namespace Detailly.Application.Modules.Booking.Locations.Commands.Create;

public sealed class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.TotalBays)
            .GreaterThan(0)
            .WithMessage("Total bays must be greater than zero.");

        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Address is required.");

        RuleFor(x => x.Address.Street)
            .NotEmpty()
            .WithMessage("Street is required.")
            .MaximumLength(200)
            .WithMessage("Street cannot exceed 200 characters.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.PostalCode)
            .NotEmpty()
            .WithMessage("Postal code is required.")
            .MaximumLength(20)
            .WithMessage("Postal code cannot exceed 20 characters.")
            .When(x => x.Address is not null);

        RuleFor(x => x.Address.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters.")
            .When(x => x.Address is not null);

        RuleForEach(x => x.OpeningHours)
            .ChildRules(h =>
            {
                h.RuleFor(x => x.DayOfWeek)
                    .InclusiveBetween(0, 6)
                    .WithMessage("Day of week must be between 0 (Sunday) and 6 (Saturday).");

                h.RuleFor(x => x.OpenHour)
                    .InclusiveBetween(0, 23)
                    .When(x => !x.IsClosed && x.OpenHour.HasValue)
                    .WithMessage("Open hour must be between 0 and 23.");

                h.RuleFor(x => x.OpenMinute)
                    .InclusiveBetween(0, 59)
                    .When(x => !x.IsClosed && x.OpenMinute.HasValue)
                    .WithMessage("Open minute must be between 0 and 59.");

                h.RuleFor(x => x.CloseHour)
                    .InclusiveBetween(0, 23)
                    .When(x => !x.IsClosed && x.CloseHour.HasValue)
                    .WithMessage("Close hour must be between 0 and 23.");

                h.RuleFor(x => x.CloseMinute)
                    .InclusiveBetween(0, 59)
                    .When(x => !x.IsClosed && x.CloseMinute.HasValue)
                    .WithMessage("Close minute must be between 0 and 59.");
            })
            .When(x => x.OpeningHours is not null);
    }
}
