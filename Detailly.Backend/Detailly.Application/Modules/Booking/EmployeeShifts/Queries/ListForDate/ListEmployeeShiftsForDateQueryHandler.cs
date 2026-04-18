namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;

public sealed class ListEmployeeShiftsForDateQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListEmployeeShiftsForDateQuery, PageResult<ListEmployeeShiftsForDateQueryDto>>
{
    public async Task<PageResult<ListEmployeeShiftsForDateQueryDto>> Handle(ListEmployeeShiftsForDateQuery request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!currentUser.IsAdmin && !currentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only Admin/Manager can view shifts.");

        var date = request.DateUtc.Date;
        var dayStart = date;
        var dayEnd = date.AddDays(1);

        var q = context.EmployeeShifts.AsNoTracking();

        var projecetedQuery = q
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.EndUtc > dayStart &&
                s.StartUtc < dayEnd);

        if (request.EmployeeWorkMode is not null)
            projecetedQuery = projecetedQuery.Where(s => s.EmployeeWorkMode == request.EmployeeWorkMode.Value);

        var result = projecetedQuery
            .OrderBy(s => s.StartUtc)
            .Select(s => new ListEmployeeShiftsForDateQueryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                ShopLocationId = s.ShopLocationId,
                EmployeeWorkMode = s.EmployeeWorkMode,
                StartUtc = s.StartUtc,
                EndUtc = s.EndUtc
            });

        return await PageResult<ListEmployeeShiftsForDateQueryDto>.FromQueryableAsync(result, request.Paging, ct);
    }
}
