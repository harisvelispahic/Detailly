
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAvailabilityQuery, List<GetAvailabilityQueryDto>>
{
    private static readonly TimeSpan DayStart = new(8, 0, 0);
    private static readonly TimeSpan DayEnd = new(20, 0, 0); // last start will be (DayEnd - duration)

    public async Task<List<GetAvailabilityQueryDto>> Handle(GetAvailabilityQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // validate package exists
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        // base totals
        var baseDuration = package.BaseDurationMinutes ?? 0;
        var baseEmployees = package.BaseRequiredEmployees ?? 1;

        // addon validation + totals
        var addonIds = (request.AddonItemIds ?? new List<int>())
            .Distinct()
            .ToList();

        // prevent selecting addon that exists in package base items
        if (addonIds.Count > 0)
        {
            var baseItemIds = await context.ServicePackageItemAssignments
                .Where(a => a.ServicePackageId == request.ServicePackageId && !a.IsDeleted)
                .Select(a => a.ServicePackageItemId)
                .ToListAsync(ct);

            if (addonIds.Intersect(baseItemIds).Any())
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_DUPLICATE",
                    "One or more add-ons are already included in the selected package.");
        }

        int addonsDuration = 0;
        int addonsEmployeesMax = 0;

        if (addonIds.Count > 0)
        {
            var addons = await context.ServicePackageItems
                .Where(i => addonIds.Contains(i.Id) && !i.IsDeleted && i.IsAddon && i.IsActive)
                .ToListAsync(ct);

            if (addons.Count != addonIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_INVALID",
                    "One or more add-on items are invalid or inactive.");

            addonsDuration = addons.Sum(x => x.DurationMinutes);
            addonsEmployeesMax = addons.Max(x => x.RequiredEmployees);
        }

        var totalDurationMinutes = baseDuration + addonsDuration;
        if (totalDurationMinutes <= 0)
            throw new DetaillyBusinessRuleException("BOOKING_DURATION_INVALID",
                "Package duration is invalid.");

        var requiredEmployees = Math.Max(baseEmployees, addonsEmployeesMax);
        var requiredBays = request.ServiceMode == ServiceMode.InShop ? 1 : 0;

        // build day window
        var date = request.DateUtc.Date;
        var windowStart = date.Add(DayStart);
        var windowEnd = date.Add(DayEnd);

        // prefetch bookings that can overlap this day (for performance)
        var blockingBookings = await context.Bookings
            .Where(b =>
                !b.IsDeleted &&
                b.ShopLocationId == request.ShopLocationId &&
                b.ServiceMode == request.ServiceMode &&
                (
                    b.Status == BookingStatus.Confirmed ||
                    (b.Status == BookingStatus.PendingPayment && b.ReservationExpiresAtUtc != null && b.ReservationExpiresAtUtc > now)
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

        // prefetch shifts that can cover (filter by mode)
        var shifts = await context.EmployeeShifts
            .Where(s =>
                s.ShopLocationId == request.ShopLocationId &&
                s.EmployeeWorkMode == (request.ServiceMode == ServiceMode.InShop ? EmployeeWorkMode.InShop : EmployeeWorkMode.Mobile) &&
                s.EndUtc > windowStart &&
                s.StartUtc < windowEnd
            )
            .Select(s => new { s.EmployeeId, s.StartUtc, s.EndUtc })
            .ToListAsync(ct);

        var results = new List<GetAvailabilityQueryDto>();

        // candidate start times every 30 minutes
        for (var start = windowStart; start < windowEnd; start = start.AddMinutes(30))
        {
            var end = start.AddMinutes(totalDurationMinutes);
            if (end > windowEnd) break;

            // supply: employees with shift fully covering interval
            var availableEmployees = shifts
                .Where(s => s.StartUtc <= start && s.EndUtc >= end)
                .Select(s => s.EmployeeId)
                .Distinct()
                .Count();

            // demand: overlapping bookings
            var usedEmployees = blockingBookings
                .Where(b => b.StartUtc < end && b.EndUtc > start)
                .Sum(b => b.RequiredEmployees);

            var freeEmployees = availableEmployees - usedEmployees;
            if (freeEmployees < requiredEmployees) continue;

            if (request.ServiceMode == ServiceMode.InShop)
            {
                var totalBays = await context.Locations
                    .Where(l => l.Id == request.ShopLocationId && !l.IsDeleted)
                    .Select(l => l.TotalBays)
                    .FirstOrDefaultAsync(ct);

                var usedBays = blockingBookings
                    .Where(b => b.StartUtc < end && b.EndUtc > start)
                    .Sum(b => b.RequiredBays);

                var freeBays = totalBays - usedBays;
                if (freeBays < requiredBays) continue;
            }

            // Don’t show times in the past (optional UX)
            if (start < now) continue;

            results.Add(new GetAvailabilityQueryDto { StartUtc = start, EndUtc = end });
        }

        return results;
    }
}