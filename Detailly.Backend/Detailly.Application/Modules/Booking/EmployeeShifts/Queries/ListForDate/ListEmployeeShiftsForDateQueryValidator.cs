namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;

public sealed class ListEmployeeShiftsForDateQueryValidator : AbstractValidator<ListEmployeeShiftsForDateQuery>
{
    public ListEmployeeShiftsForDateQueryValidator()
    {
        RuleFor(x => x.DateUtc)
            .Must(d => d != default(DateTime))
            .WithMessage("DateUtc must be provided (date part will be used).");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location id must be greater than 0.");
    }
}
