using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMyAssigned;

public sealed class ListMyAssignedBookingsQueryHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<ListMyAssignedBookingsQuery, PageResult<ListMyAssignedBookingsQueryDto>>
{
    public async Task<PageResult<ListMyAssignedBookingsQueryDto>> Handle(
        ListMyAssignedBookingsQuery request,
        CancellationToken ct)
    {
        authService.EnsureEmployee();
        var employeeId = authService.RequireUserId();

        var projectedQuery = context.BookingEmployeeAssignments
            .AsNoTracking()
            .Where(a =>
                !a.IsDeleted &&
                a.EmployeeId == employeeId &&
                !a.Booking.IsDeleted &&
                (a.Booking.Status == BookingStatus.Confirmed || a.Booking.Status == BookingStatus.Completed))
            .OrderByDescending(a => a.Booking.StartUtc)
            .Select(a => new ListMyAssignedBookingsQueryDto
            {
                Id = a.Booking.Id,
                Status = a.Booking.Status,
                ServiceMode = a.Booking.ServiceMode,
                StartUtc = a.Booking.StartUtc,
                EndUtc = a.Booking.EndUtc,
                TotalPrice = a.Booking.TotalPrice,
                ServicePackageName = a.Booking.ServicePackage.Name,
                CustomerName = a.Booking.Customer.FirstName + " " + a.Booking.Customer.LastName,
                ShopLocationName = a.Booking.ShopLocation.Name,
                Notes = a.Booking.Notes
            });

        return await PageResult<ListMyAssignedBookingsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
