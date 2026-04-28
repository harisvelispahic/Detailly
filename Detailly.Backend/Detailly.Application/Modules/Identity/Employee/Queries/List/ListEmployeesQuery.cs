using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Identity.Employee.Queries.List;

public sealed class ListEmployeesQuery : IRequest<List<ListEmployeesQueryDto>>
{
    public EmployeeWorkMode? EmployeeWorkMode { get; init; }
}
