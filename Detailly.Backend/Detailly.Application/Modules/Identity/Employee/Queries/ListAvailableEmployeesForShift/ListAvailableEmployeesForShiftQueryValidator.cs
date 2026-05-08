namespace Detailly.Application.Modules.Identity.Employee.Queries.ListAvailableEmployeesForShift;

public sealed class ListAvailableEmployeesForShiftQueryValidator : AbstractValidator<ListAvailableEmployeesForShiftQuery>
{
    public ListAvailableEmployeesForShiftQueryValidator()
    {
        RuleFor(x => x.ExcludeShiftId)
            .GreaterThan(0)
            .When(x => x.ExcludeShiftId.HasValue)
            .WithMessage("Exclude shift ID must be greater than zero.");
    }
}
