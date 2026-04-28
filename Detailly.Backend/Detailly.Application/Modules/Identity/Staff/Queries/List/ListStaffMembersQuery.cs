namespace Detailly.Application.Modules.Identity.Staff.Queries.List;

public class ListStaffMembersQuery : BasePagedQuery<ListStaffMembersQueryDto>
{
    public string? Search { get; init; }
    /// <summary>null = all staff, true = managers only, false = employees only</summary>
    public bool? IsManager { get; init; }
}
