using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListUnassigned;

public sealed class ListUnassignedBookingsQueryHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<ListUnassignedBookingsQuery, PageResult<ListUnassignedBookingsQueryDto>>
{
    public async Task<PageResult<ListUnassignedBookingsQueryDto>> Handle(
        ListUnassignedBookingsQuery request,
        CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

        var projectedQuery = context.Bookings
            .AsNoTracking()
            .Where(b =>
                !b.IsDeleted &&
                b.Status == BookingStatus.Confirmed &&
                !b.EmployeeAssignments.Any(a => !a.IsDeleted))
            .OrderBy(b => b.StartUtc)
            .Select(b => new ListUnassignedBookingsQueryDto
            {
                Id = b.Id,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                RequiredEmployees = b.RequiredEmployees,
                ServiceMode = b.ServiceMode,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName,
                ServicePackageName = b.ServicePackage.Name,
                TotalPrice = b.TotalPrice,
                Notes = b.Notes
            });

        return await PageResult<ListUnassignedBookingsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
