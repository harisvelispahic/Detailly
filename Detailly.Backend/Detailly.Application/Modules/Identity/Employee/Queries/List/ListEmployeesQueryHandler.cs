namespace Detailly.Application.Modules.Identity.Employee.Queries.List;

public sealed class ListEmployeesQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListEmployeesQuery, List<ListEmployeesQueryDto>>
{
    public async Task<List<ListEmployeesQueryDto>> Handle(ListEmployeesQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || (!currentUser.IsAdmin && !currentUser.IsManager))
            throw new DetaillyForbiddenException("Admin or Manager access required.");

        var q = context.ApplicationUsers
            .AsNoTracking()
            .Where(u => u.IsEmployee && u.IsEnabled && !u.IsDeleted);

        if (request.EmployeeWorkMode.HasValue)
            q = q.Where(u => u.EmployeeWorkMode == request.EmployeeWorkMode.Value
                             || u.EmployeeWorkMode == null);

        return await q
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new ListEmployeesQueryDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
                EmployeeWorkMode = u.EmployeeWorkMode,
            })
            .ToListAsync(ct);
    }
}
