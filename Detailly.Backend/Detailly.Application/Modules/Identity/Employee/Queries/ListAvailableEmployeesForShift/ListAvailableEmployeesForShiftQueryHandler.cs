namespace Detailly.Application.Modules.Identity.Employee.Queries.ListAvailableEmployeesForShift;

public sealed class ListAvailableEmployeesForShiftQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListAvailableEmployeesForShiftQuery, PageResult<ListAvailableEmployeesForShiftQueryDto>>
{
    public async Task<PageResult<ListAvailableEmployeesForShiftQueryDto>> Handle(ListAvailableEmployeesForShiftQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || (!currentUser.IsAdmin && !currentUser.IsManager))
            throw new DetaillyForbiddenException("Admin or Manager access required.");

        var query = context.ApplicationUsers
            .AsNoTracking()
            .Where(u => u.IsEmployee && u.IsEnabled && !u.IsDeleted);

        if (request.DateUtc.HasValue)
        {
            var dateStart = request.DateUtc.Value.Date;
            var dateEnd = dateStart.AddDays(1);

            var busyIds = context.EmployeeShifts
                .Where(s => !s.IsDeleted && s.StartUtc < dateEnd && s.EndUtc > dateStart);

            if (request.ExcludeShiftId.HasValue)
                busyIds = busyIds.Where(s => s.Id != request.ExcludeShiftId.Value);

            var busyEmployeeIds = busyIds.Select(s => s.EmployeeId);
            query = query.Where(u => !busyEmployeeIds.Contains(u.Id));
        }

        var projected = query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new ListAvailableEmployeesForShiftQueryDto
            {
                Id = u.Id,
                FullName = u.FirstName + " " + u.LastName,
            });

        return await PageResult<ListAvailableEmployeesForShiftQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}
