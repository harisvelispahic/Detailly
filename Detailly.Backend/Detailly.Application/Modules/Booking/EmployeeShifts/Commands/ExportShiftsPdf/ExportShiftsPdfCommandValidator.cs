namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.ExportShiftsPdf;

public sealed class ExportShiftsPdfCommandValidator : AbstractValidator<ExportShiftsPdfCommand>
{
    public ExportShiftsPdfCommandValidator()
    {
        RuleFor(x => x.StartDateUtc)
            .Must(d => d != default)
            .WithMessage("Start date must be provided.");

        RuleFor(x => x.EndDateUtc)
            .Must(d => d != default)
            .WithMessage("End date must be provided.");

        RuleFor(x => x)
            .Must(x => x.EndDateUtc >= x.StartDateUtc)
            .WithMessage("End date must not be before start date.");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location ID must be greater than zero.");
    }
}
