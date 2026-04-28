namespace Detailly.Application.Modules.Identity.Staff.Queries.List;

public class ListStaffMembersQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public required bool IsManager { get; init; }
}
