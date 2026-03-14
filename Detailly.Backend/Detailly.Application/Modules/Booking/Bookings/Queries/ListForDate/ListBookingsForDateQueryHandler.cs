using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;

public sealed class ListBookingsForDateQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListBookingsForDateQuery, List<ListBookingsForDateQueryDto>>
{
    public async Task<List<ListBookingsForDateQueryDto>> Handle(ListBookingsForDateQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        // Staff-only (simple gate for now). Tighten later with policies/locations if you want.
        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager && !appCurrentUser.IsEmployee)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Staff access required.");

        var date = request.DateUtc.Date;
        var dayStart = date;
        var dayEnd = date.AddDays(1);

        // Status filter:
        // - always include Confirmed/Completed
        // - optionally include PendingPayment, but only if hold still active
        var includePending = request.IncludePendingPaymentHolds;

        var query = context.Bookings
            .AsNoTracking()
            .Where(b =>
                !b.IsDeleted &&
                b.ShopLocationId == request.ShopLocationId &&
                b.ServiceMode == request.ServiceMode &&
                b.StartUtc >= dayStart &&
                b.StartUtc < dayEnd &&
                (
                    b.Status == BookingStatus.Confirmed ||
                    b.Status == BookingStatus.Completed ||
                    (includePending &&
                     b.Status == BookingStatus.PendingPayment &&
                     b.ReservationExpiresAtUtc != null &&
                     b.ReservationExpiresAtUtc > now)
                ))
            .OrderBy(b => b.StartUtc)
            .Select(b => new ListBookingsForDateQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                RequiredEmployees = b.RequiredEmployees,
                RequiredBays = b.RequiredBays,
                Notes = b.Notes,
                ReservationExpiresAtUtc = b.ReservationExpiresAtUtc,

                // Customer + package info
                CustomerName = (b.Customer.FirstName + " " + b.Customer.LastName),
                ServicePackageName = b.ServicePackage.Name,

                // Assigned employees (if none yet, list is empty)
                AssignedEmployees = b.EmployeeAssignments
                    .OrderBy(a => a.EmployeeId)
                    .Select(a => new ListBookingsForDateAssignedEmployeeDto
                    {
                        EmployeeId = a.EmployeeId,
                        FullName = (a.Employee.FirstName + " " + a.Employee.LastName)
                    })
                    .ToList()
            });

        return await query.ToListAsync(ct);
    }
}