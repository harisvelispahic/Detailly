namespace Detailly.Application.Modules.Booking.Locations.Commands.Delete;

public sealed class DeleteLocationCommandValidator : AbstractValidator<DeleteLocationCommand>
{
    public DeleteLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than zero.");
    }
}
