namespace Detailly.Application.Modules.Identity.Employee.Queries.List;

public sealed class ListEmployeesQueryDto
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
}
