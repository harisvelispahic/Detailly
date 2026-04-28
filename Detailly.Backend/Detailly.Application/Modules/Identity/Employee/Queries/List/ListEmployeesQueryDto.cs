using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Identity.Employee.Queries.List;

public sealed class ListEmployeesQueryDto
{
    public required int Id { get; init; }
    public required string FullName { get; init; }
    public EmployeeWorkMode? EmployeeWorkMode { get; init; }
}
