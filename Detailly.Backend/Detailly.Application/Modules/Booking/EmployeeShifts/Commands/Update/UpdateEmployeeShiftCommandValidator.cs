namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;

public sealed class UpdateEmployeeShiftCommandValidator : AbstractValidator<UpdateEmployeeShiftCommand>
{
    public UpdateEmployeeShiftCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Shift id must be greater than 0.");

        When(x => x.EmployeeId != null, () =>
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Employee id must be greater than 0.");
        });

        When(x => x.ShopLocationId != null, () =>
        {
            RuleFor(x => x.ShopLocationId)
                .GreaterThan(0)
                .WithMessage("Shop location id must be greater than 0.");
        });

        // Only validate ordering if both provided
        When(x => x.StartUtc != null && x.EndUtc != null, () =>
        {
            RuleFor(x => x)
                .Must(x => x.EndUtc!.Value > x.StartUtc!.Value)
                .WithMessage("Shift end must be after shift start.");
        });
    }
}
