namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListMine;

public sealed class ListMyShiftsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ListMyShiftsQuery, List<ListMyShiftsQueryDto>>
{
    public async Task<List<ListMyShiftsQueryDto>> Handle(ListMyShiftsQuery request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!currentUser.IsEmployee)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only employees can view their own shifts.");

        var today = DateTime.UtcNow.Date;
        var rangeStart = today.AddDays(-request.Days);
        var rangeEnd = today.AddDays(request.Days + 1);

        return await context.EmployeeShifts.AsNoTracking()
            .Where(s =>
                !s.IsDeleted &&
                s.EmployeeId == currentUser.ApplicationUserId.Value &&
                s.EndUtc > rangeStart &&
                s.StartUtc < rangeEnd)
            .OrderBy(s => s.StartUtc)
            .Select(s => new ListMyShiftsQueryDto
            {
                Id = s.Id,
                ShopLocationId = s.ShopLocationId,
                ShopLocationName = s.ShopLocation.Name,
                EmployeeWorkMode = s.EmployeeWorkMode,
                StartUtc = s.StartUtc,
                EndUtc = s.EndUtc
            })
            .ToListAsync(ct);
    }
}
