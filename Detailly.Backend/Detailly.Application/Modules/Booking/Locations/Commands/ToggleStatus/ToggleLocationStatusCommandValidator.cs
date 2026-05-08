namespace Detailly.Application.Modules.Booking.Locations.Commands.ToggleStatus;

public sealed class ToggleLocationStatusCommandValidator : AbstractValidator<ToggleLocationStatusCommand>
{
    public ToggleLocationStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than zero.");
    }
}
