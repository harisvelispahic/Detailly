using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQueryHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IBookingQuoteService quoteService,
    TimeProvider timeProvider)
    : IRequestHandler<GetAvailabilityQuery, GetAvailabilityResult>
{
    public async Task<GetAvailabilityResult> Handle(GetAvailabilityQuery request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var vehicleIds = (request.VehicleIds ?? new List<int>())
            .Distinct()
            .ToList();

        var hasVehicles = vehicleIds.Count > 0;
        var customerId  = hasVehicles ? appCurrentUser.ApplicationUserId : (int?)null;
        var isFleet     = hasVehicles && appCurrentUser.IsFleet;

        // 1) Quote — drives duration, employee base, bays, travel time, price
        var quote = await quoteService.CalculateAsync(
            request.ServicePackageId,
            request.AddonItemIds,
            request.ServiceMode,
            hasVehicles ? vehicleIds : null,
            customerId,
            isFleet,
            ct,
            serviceAddressId: request.ServiceAddressId,
            shopLocationId: request.ServiceAddressId.HasValue ? request.ShopLocationId : null);

        var travelTimeMinutes    = quote.TravelTimeMinutes;
        var totalDurationMinutes = quote.TotalDurationMinutes;
        var requiredEmployees    = quote.RequiredEmployees;
        var requiredBays         = quote.RequiredBays;

        // Fleet mobile multi-vehicle: k-optimisation runs per slot (maxShiftEnd varies).
        var vehicleCount             = Math.Max(1, vehicleIds.Count);
        var isFleetMobileMultiVehicle = isFleet && request.ServiceMode == ServiceMode.Mobile && vehicleCount > 1;

        // 2) Opening hours (per location + day)
        var date      = request.DateUtc.Date;
        var localDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(date, DateTimeKind.Utc), LocationOpeningHoursEntity.LocalZone).Date;
        var dayOfWeek = (int)localDate.DayOfWeek; // Sunday=0 ... Saturday=6

        var opening = await context.LocationOpeningHours
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.ShopLocationId == request.ShopLocationId &&
                x.DayOfWeek == dayOfWeek,
                ct);

        if (opening is null || opening.IsClosed)
            return new GetAvailabilityResult
            {
                TravelTimeMinutes  = travelTimeMinutes,
                MobileSurchargeFee = quote.MobileSurchargeFee,
            };

        var openTime  = opening.OpenTime  ?? LocationOpeningHoursEntity.DefaultOpenTime;
        var closeTime = opening.CloseTime ?? LocationOpeningHoursEntity.DefaultCloseTime;

        // Opening hours are local time; convert to UTC for slot generation.
        var windowStart = TimeZoneInfo.ConvertTimeToUtc(localDate.Add(openTime),  LocationOpeningHoursEntity.LocalZone);
        var windowEnd   = TimeZoneInfo.ConvertTimeToUtc(localDate.Add(closeTime), LocationOpeningHoursEntity.LocalZone);

        if (windowEnd <= windowStart)
            return new GetAvailabilityResult
            {
                TravelTimeMinutes  = travelTimeMinutes,
                MobileSurchargeFee = quote.MobileSurchargeFee,
            };

        // 3) Fetch total bays ONCE if InShop
        int totalBays = 0;
        if (request.ServiceMode == ServiceMode.InShop)
        {
            totalBays = await context.Locations
                .AsNoTracking()
                .Where(l => l.Id == request.ShopLocationId && !l.IsDeleted)
                .Select(l => l.TotalBays)
                .FirstOrDefaultAsync(ct);

            if (totalBays <= 0)
                return new GetAvailabilityResult
                {
                    TravelTimeMinutes  = travelTimeMinutes,
                    MobileSurchargeFee = quote.MobileSurchargeFee,
                };
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
                b.EndUtc   > windowStart &&
                b.StartUtc < windowEnd
            )
            .Select(b => new
            {
                b.StartUtc,
                b.EndUtc,
                b.RequiredEmployees,
                b.RequiredBays,
                b.TravelTimeMinutes
            })
            .ToListAsync(ct);

        // 5) Prefetch shifts (filter by mode).
        // For mobile, a shift must cover [departure, return] where departure = start - travel.
        // The earliest departure is windowStart - travelTimeMinutes, so widen the fetch window.
        var shiftFetchStart = windowStart.AddMinutes(-travelTimeMinutes);
        var shiftMode = request.ServiceMode == ServiceMode.InShop
            ? EmployeeWorkMode.InShop
            : EmployeeWorkMode.Mobile;

        var shifts = await context.EmployeeShifts
            .AsNoTracking()
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.EndUtc   > shiftFetchStart &&
                s.StartUtc < windowEnd
            )
            .Select(s => new { s.EmployeeId, s.StartUtc, s.EndUtc })
            .ToListAsync(ct);

        var results = new List<GetAvailabilityQueryDto>();

        // 6) Generate candidate start times every 30 minutes
        for (var start = windowStart; start < windowEnd; start = start.AddMinutes(30))
        {
            if (start < now) continue;

            var departure = start.AddMinutes(-travelTimeMinutes);

            var slotRequiredEmployees    = requiredEmployees;
            var slotTotalDurationMinutes = totalDurationMinutes;

            // For fleet mobile multi-vehicle, recompute capacity per slot because maxShiftEnd
            // (and therefore optimal k and duration) differs for each candidate start time.
            if (isFleetMobileMultiVehicle)
            {
                var maxShiftEnd = shifts
                    .Where(s => s.StartUtc <= departure && s.EndUtc > start)
                    .Select(s => s.EndUtc)
                    .DefaultIfEmpty()
                    .Max();

                if (maxShiftEnd == default) continue;

                var capacity = quoteService.ComputeFleetMobileCapacity(
                    vehicleCount,
                    quote.RequiredEmployees,
                    quote.PerVehicleDurationMinutes,
                    start,
                    travelTimeMinutes,
                    maxShiftEnd);

                if (capacity is null) continue;

                slotRequiredEmployees    = capacity.RequiredEmployees;
                slotTotalDurationMinutes = capacity.TotalDurationMinutes;
            }

            var end = start.AddMinutes(slotTotalDurationMinutes);

            if (end > windowEnd)
            {
                // For fleet mobile, duration can shrink for later slots (more employees fit the remaining window),
                // so continue rather than break. For fixed-duration cases, break is safe.
                if (isFleetMobileMultiVehicle)
                    continue;
                else
                    break;
            }

            var returnTime = end.AddMinutes(travelTimeMinutes);

            // Supply: employees whose shift fully covers the effective window [departure, returnTime]
            var availableEmployees = shifts
                .Where(s => s.StartUtc <= departure && s.EndUtc >= returnTime)
                .Select(s => s.EmployeeId)
                .Distinct()
                .Count();

            // Demand: blockers whose effective window intersects [departure, returnTime]
            var usedEmployees = blockingBookings
                .Where(b =>
                {
                    var bDep = b.StartUtc.AddMinutes(-(b.TravelTimeMinutes ?? 0));
                    var bRet = b.EndUtc.AddMinutes(b.TravelTimeMinutes ?? 0);
                    return bDep < returnTime && bRet > departure;
                })
                .Sum(b => b.RequiredEmployees);

            if (availableEmployees - usedEmployees < slotRequiredEmployees)
                continue;

            if (request.ServiceMode == ServiceMode.InShop)
            {
                var usedBays = blockingBookings
                    .Where(b =>
                    {
                        var bDep = b.StartUtc.AddMinutes(-(b.TravelTimeMinutes ?? 0));
                        var bRet = b.EndUtc.AddMinutes(b.TravelTimeMinutes ?? 0);
                        return bDep < returnTime && bRet > departure;
                    })
                    .Sum(b => b.RequiredBays);

                if (totalBays - usedBays < requiredBays)
                    continue;
            }

            results.Add(new GetAvailabilityQueryDto
            {
                StartUtc = start,
                EndUtc   = end,
            });
        }

        return new GetAvailabilityResult
        {
            Slots              = results,
            TravelTimeMinutes  = travelTimeMinutes,
            MobileSurchargeFee = quote.MobileSurchargeFee,
        };
    }
}
