namespace Detailly.Application.Modules.Identity.Employee.Queries.List;

public sealed class ListEmployeesQuery : BasePagedQuery<ListEmployeesQueryDto>
{
    /// <summary>When provided, excludes employees who already have a shift on this UTC date.</summary>
    public DateTime? DateUtc { get; init; }

    /// <summary>Shift ID to exclude from the conflict check (used in edit mode).</summary>
    public int? ExcludeShiftId { get; init; }
}
