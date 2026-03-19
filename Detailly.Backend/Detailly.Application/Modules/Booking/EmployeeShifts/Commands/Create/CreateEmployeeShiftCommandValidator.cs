namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;

public sealed class CreateEmployeeShiftCommandValidator : AbstractValidator<CreateEmployeeShiftCommand>
{
    public CreateEmployeeShiftCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee id must be greater than 0.");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location id must be greater than 0.");

        RuleFor(x => x.StartUtc)
            .Must(d => d != default(DateTime))
            .WithMessage("StartUtc must be set.");

        RuleFor(x => x.EndUtc)
            .Must(d => d != default(DateTime))
            .WithMessage("EndUtc must be set.");

        RuleFor(x => x)
            .Must(x => x.EndUtc > x.StartUtc)
            .WithMessage("Shift end must be after shift start.");
    }
}
