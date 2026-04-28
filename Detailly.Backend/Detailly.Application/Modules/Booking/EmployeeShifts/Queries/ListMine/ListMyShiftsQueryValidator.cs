namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListMine;

public sealed class ListMyShiftsQueryValidator : AbstractValidator<ListMyShiftsQuery>
{
    public ListMyShiftsQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(1, 30)
            .WithMessage("Days must be between 1 and 30.");
    }
}
