using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQueryHandler(
    IAppDbContext context,
    IBookingQuoteService quoteService)
    : IRequestHandler<GetAvailabilityQuery, List<GetAvailabilityQueryDto>>
{
    public async Task<List<GetAvailabilityQueryDto>> Handle(GetAvailabilityQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // 1) Quote (single source of truth for duration/employees/bays)
        var quote = await quoteService.CalculateAsync(
            request.ServicePackageId,
            request.AddonItemIds,
            request.ServiceMode,
            vehicleIds: null,
            customerId: null,
            isFleet: false,
            ct,
            serviceAddressId: null,
            shopLocationId: null);

        // 2) Opening hours (per location + day)
        var date = request.DateUtc.Date;
        var dayOfWeek = (int)date.DayOfWeek; // Sunday=0 ... Saturday=6

        var opening = await context.LocationOpeningHours
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.ShopLocationId == request.ShopLocationId &&
                x.DayOfWeek == dayOfWeek,
                ct);

        // If no row exists OR explicitly closed -> no availability
        if (opening is null || opening.IsClosed)
            return new List<GetAvailabilityQueryDto>();

        // Build the daily window from opening hours, if missing assume 8am-8pm UTC (can be configured per location + day)
        var windowStart = date.Add(opening.OpenTimeUtc ?? new TimeSpan(8, 0, 0));
        var windowEnd = date.Add(opening.CloseTimeUtc ?? new TimeSpan(20, 0, 0));

        // Safety: invalid config
        if (windowEnd <= windowStart)
            return new List<GetAvailabilityQueryDto>();

        // For convenience
        var totalDurationMinutes = quote.TotalDurationMinutes;
        var requiredEmployees = quote.RequiredEmployees;
        var requiredBays = quote.RequiredBays;

        // 3) Fetch total bays ONCE if InShop
        int totalBays = 0;
        if (request.ServiceMode == ServiceMode.InShop)
        {
            totalBays = await context.Locations
                .AsNoTracking()
                .Where(l => l.Id == request.ShopLocationId && !l.IsDeleted)
                .Select(l => l.TotalBays)
                .FirstOrDefaultAsync(ct);

            // If location missing or configured as 0 bays -> no availability
            if (totalBays <= 0)
                return new List<GetAvailabilityQueryDto>();
        }

        // 4) Prefetch blocking bookings for the day-window
        var blockingBookings = await context.Bookings
            .AsNoTracking()
            .Where(b =>
                !b.IsDeleted &&
                b.ShopLocationId == request.ShopLocationId &&
                b.ServiceMode == request.ServiceMode &&
                (
                    b.Status == BookingStatus.Confirmed ||
                    (b.Status == BookingStatus.PendingPayment &&
                     b.ReservationExpiresAtUtc != null &&
                     b.ReservationExpiresAtUtc > now)
                ) &&
                b.EndUtc > windowStart &&
                b.StartUtc < windowEnd
            )
            .Select(b => new
            {
                b.StartUtc,
                b.EndUtc,
                b.RequiredEmployees,
                b.RequiredBays
            })
            .ToListAsync(ct);

        // 5) Prefetch shifts (filter by mode)
        var shifts = await context.EmployeeShifts
            .AsNoTracking()
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.EmployeeWorkMode == (request.ServiceMode == ServiceMode.InShop
                    ? EmployeeWorkMode.InShop
                    : EmployeeWorkMode.Mobile) &&
                s.EndUtc > windowStart &&
                s.StartUtc < windowEnd
            )
            .Select(s => new { s.EmployeeId, s.StartUtc, s.EndUtc })
            .ToListAsync(ct);

        var results = new List<GetAvailabilityQueryDto>();

        // 6) Generate candidate start times every 30 minutes
        for (var start = windowStart; start < windowEnd; start = start.AddMinutes(30))
        {
            // Optional UX: don't show past starts
            if (start < now) continue;

            var end = start.AddMinutes(totalDurationMinutes);
            if (end > windowEnd) break;

            // Supply: employees whose shift fully covers [start, end)
            var availableEmployees = shifts
                .Where(s => s.StartUtc <= start && s.EndUtc >= end)
                .Select(s => s.EmployeeId)
                .Distinct()
                .Count();

            // Demand: employees used by overlapping blocking bookings
            var usedEmployees = blockingBookings
                .Where(b => b.StartUtc < end && b.EndUtc > start)
                .Sum(b => b.RequiredEmployees);

            if (availableEmployees - usedEmployees < requiredEmployees)
                continue;

            if (request.ServiceMode == ServiceMode.InShop)
            {
                var usedBays = blockingBookings
                    .Where(b => b.StartUtc < end && b.EndUtc > start)
                    .Sum(b => b.RequiredBays);

                if (totalBays - usedBays < requiredBays)
                    continue;
            }

            results.Add(new GetAvailabilityQueryDto
            {
                StartUtc = start,
                EndUtc = end
            });
        }

        return results;
    }
}