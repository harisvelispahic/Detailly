namespace Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;

public sealed class CreateBookingHoldCommandValidator : AbstractValidator<CreateBookingHoldCommand>
{
    public CreateBookingHoldCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location ID must be greater than zero.");

        RuleFor(x => x.StartUtc)
            .Must(d => d != default)
            .WithMessage("Start date/time must be provided.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 500 characters.");

        RuleForEach(x => x.AddonItemIds)
            .GreaterThan(0)
            .When(x => x.AddonItemIds is not null)
            .WithMessage("Each add-on item ID must be greater than zero.");

        RuleForEach(x => x.VehicleIds)
            .GreaterThan(0)
            .When(x => x.VehicleIds is not null)
            .WithMessage("Each vehicle ID must be greater than zero.");
    }
}
