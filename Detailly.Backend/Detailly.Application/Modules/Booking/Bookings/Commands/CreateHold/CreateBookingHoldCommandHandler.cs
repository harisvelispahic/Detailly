using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;

public sealed class CreateBookingHoldCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IBookingQuoteService quoteService)
    : IRequestHandler<CreateBookingHoldCommand, int>
{
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(10);

    public async Task<int> Handle(CreateBookingHoldCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        if (appCurrentUser.IsFleet && request.ServiceMode == ServiceMode.InShop)
            throw new DetaillyBusinessRuleException("FLEET_INSHOP_NOT_ALLOWED",
                "Fleet customers can only book mobile (on-address) services.");

        // Fix 2: explicit switch so an unknown ServiceMode surfaces immediately
        var shiftMode = request.ServiceMode switch
        {
            ServiceMode.InShop  => EmployeeWorkMode.InShop,
            ServiceMode.Mobile  => EmployeeWorkMode.Mobile,
            _ => throw new DetaillyBusinessRuleException("BOOKING_INVALID_MODE", "Unsupported service mode.")
        };

        if (request.StartUtc.Second != 0 || request.StartUtc.Minute % 30 != 0)
            throw new DetaillyBusinessRuleException("BOOKING_TIME_INVALID",
                "Start time must be aligned to 30-minute intervals.");

        if (request.StartUtc <= now)
            throw new DetaillyBusinessRuleException("BOOKING_TIME_PAST",
                "Start time must be in the future.");

        // ServiceMode address rules
        if (request.ServiceMode == ServiceMode.InShop)
        {
            if (request.ServiceAddressId is not null)
                throw new DetaillyBusinessRuleException("BOOKING_ADDRESS_NOT_ALLOWED",
                    "Service address must not be provided for in-shop bookings.");
        }
        else if (request.ServiceMode == ServiceMode.Mobile)
        {
            if (request.ServiceAddressId is null)
                throw new DetaillyBusinessRuleException("BOOKING_ADDRESS_REQUIRED",
                    "Service address is required for mobile bookings.");

            // Address must belong to the requesting customer
            var addressOwned = await context.Addresses
                .AnyAsync(a =>
                    a.Id == request.ServiceAddressId.Value &&
                    !a.IsDeleted &&
                    a.ApplicationUserId == customerId, ct);

            if (!addressOwned)
                throw new DetaillyBusinessRuleException("BOOKING_ADDRESS_FORBIDDEN",
                    "The service address does not belong to your account.");
        }

        // Fix 3: vehicle ownership validated before any writes
        var vehicleIds = (request.VehicleIds ?? new List<int>())
            .Distinct()
            .ToList();

        if (vehicleIds.Count > 0)
        {
            var owned = await context.Vehicles
                .Where(v =>
                    vehicleIds.Contains(v.Id) &&
                    !v.IsDeleted &&
                    v.ApplicationUserId == customerId)
                .Select(v => v.Id)
                .ToListAsync(ct);

            if (owned.Count != vehicleIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_INVALID",
                    "One or more vehicles are invalid.");
        }

        // Quote = single source of truth for price / per-vehicle duration / employee base / bays / addons
        var quote = await quoteService.CalculateAsync(
            request.ServicePackageId,
            request.AddonItemIds,
            request.ServiceMode,
            request.VehicleIds,
            customerId,
            appCurrentUser.IsFleet,
            ct,
            serviceAddressId: request.ServiceAddressId,
            shopLocationId: request.ShopLocationId);

        // -------------------------
        // Mobile fleet: k-employee optimisation
        //
        // Instead of sending one employee per vehicle (N employees, 1× duration), find the minimum
        // number of employees k such that each employee can service ceil(N/k) vehicles sequentially
        // and still return to the shop before their shift ends.
        //
        // Model (all times UTC):
        //   departureUtc  = StartUtc - travelTime
        //   returnUtc     = StartUtc + workDuration + travelTime
        //   availableWorkMinutes = maxShiftEnd - StartUtc - travelTime
        //   maxVehiclesPerEmployee = floor(availableWorkMinutes / perVehicleDuration)
        //   k = ceil(N / maxVehiclesPerEmployee)
        //   workDuration = ceil(N / k) × perVehicleDuration
        // -------------------------
        var travelTimeMinutes = quote.TravelTimeMinutes;
        var perVehicleDuration = quote.PerVehicleDurationMinutes;
        var vehicleCount = Math.Max(1, vehicleIds.Count);

        var totalDurationMinutes = quote.TotalDurationMinutes;
        var requiredEmployees = quote.RequiredEmployees;
        var requiredBays = quote.RequiredBays;

        if (appCurrentUser.IsFleet && request.ServiceMode == ServiceMode.Mobile && vehicleCount > 1)
        {
            // Query max shift end for mobile employees whose shift covers the departure window.
            // This is a pre-transaction read; the capacity check inside the transaction is the hard gate.
            var departureForOptimisation = request.StartUtc.AddMinutes(-travelTimeMinutes);

            var maxShiftEnd = await context.EmployeeShifts
                .Where(s =>
                    !s.IsDeleted &&
                    s.ShopLocationId == request.ShopLocationId &&
                    s.EmployeeWorkMode == EmployeeWorkMode.Mobile &&
                    s.StartUtc <= departureForOptimisation &&
                    s.EndUtc > request.StartUtc)
                .MaxAsync(s => (DateTime?)s.EndUtc, ct);

            if (maxShiftEnd is null)
                throw new DetaillyBusinessRuleException("BOOKING_NO_CAPACITY",
                    "No mobile employees are available for the selected time.");

            // Time employees can spend working on-site before needing to leave to return in time
            var availableWorkMinutes =
                (int)(maxShiftEnd.Value - request.StartUtc).TotalMinutes - travelTimeMinutes;

            if (availableWorkMinutes <= 0)
                throw new DetaillyBusinessRuleException("BOOKING_NO_TIME",
                    "Employees cannot complete any work and return before closing.");

            // baseEmployeesPerVehicle = how many employees are needed to service one vehicle
            // in perVehicleDuration minutes (the package's rated staffing).
            //
            // Two regimes depending on how many employees k we send:
            //
            //   k < base → partial team; work scales inversely with headcount:
            //     timePerVehicle = ceil(base × duration / k)
            //     totalDuration  = N × timePerVehicle
            //
            //   k >= base → form floor(k/base) full teams working in parallel:
            //     totalDuration  = ceil(N / teams) × duration
            //
            // We iterate from k=1 upward and take the first k whose total fits the window.
            var baseEmployeesPerVehicle = quote.RequiredEmployees;
            var maxK = vehicleCount * baseEmployeesPerVehicle;

            int? optimalK = null;
            int optimalDuration = 0;

            for (var k = 1; k <= maxK; k++)
            {
                int candidateDuration;
                if (k < baseEmployeesPerVehicle)
                {
                    var timePerVehicle = (int)Math.Ceiling((double)(baseEmployeesPerVehicle * perVehicleDuration) / k);
                    candidateDuration = vehicleCount * timePerVehicle;
                }
                else
                {
                    var teams = k / baseEmployeesPerVehicle;
                    candidateDuration = (int)Math.Ceiling((double)vehicleCount / teams) * perVehicleDuration;
                }

                if (candidateDuration <= availableWorkMinutes)
                {
                    optimalK = k;
                    optimalDuration = candidateDuration;
                    break;
                }
            }

            if (optimalK is null)
                throw new DetaillyBusinessRuleException("BOOKING_NO_TIME",
                    "Not enough time to complete all vehicles before employees must return.");

            requiredEmployees = optimalK.Value;
            totalDurationMinutes = optimalDuration;
        }

        var endUtc = request.StartUtc.AddMinutes(totalDurationMinutes);
        var totalPrice = quote.TotalPrice;

        // Effective window for capacity overlap checks (includes travel for mobile)
        var departureUtc = request.StartUtc.AddMinutes(-travelTimeMinutes);
        var returnUtc    = endUtc.AddMinutes(travelTimeMinutes);

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var location = await context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.ShopLocationId && !l.IsDeleted, ct);

        if (location is null)
            throw new DetaillyNotFoundException("Location not found.");

        // --- Vehicle conflict check (standard customers only) ---
        // Prevents a customer from booking the same vehicle in an overlapping slot.
        if (!appCurrentUser.IsEmployee && !appCurrentUser.IsManager && !appCurrentUser.IsAdmin && vehicleIds.Count > 0)
        {
            var hasVehicleConflict = await context.BookingVehicleAssignments
                .AnyAsync(bva =>
                    !bva.IsDeleted &&
                    vehicleIds.Contains(bva.VehicleId) &&
                    !bva.Booking.IsDeleted &&
                    (bva.Booking.Status == BookingStatus.Confirmed ||
                     (bva.Booking.Status == BookingStatus.PendingPayment &&
                      bva.Booking.ReservationExpiresAtUtc != null &&
                      bva.Booking.ReservationExpiresAtUtc > now)) &&
                    bva.Booking.StartUtc < endUtc &&
                    bva.Booking.EndUtc > request.StartUtc,
                    ct);

            if (hasVehicleConflict)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_CONFLICT",
                    "One or more of your vehicles already have a booking in the selected time slot. Please choose the next available slot.");
        }

        // --- Capacity check ---
        // For mobile: shifts must cover the full away period [departureUtc, returnUtc].
        // For InShop: travelTimeMinutes = 0, so departureUtc = StartUtc and returnUtc = endUtc.
        var availableEmployees = await context.EmployeeShifts
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.StartUtc <= departureUtc &&
                s.EndUtc   >= returnUtc)
            .Select(s => s.EmployeeId)
            .Distinct()
            .CountAsync(ct);

        // Existing mobile bookings block employees for their own [departure, return] window.
        // Fetch with a generous SQL filter then refine in-memory to avoid SQL DateTime arithmetic.
        var candidateBlockers = await context.Bookings
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
                b.StartUtc < returnUtc &&   // conservative pre-filter
                b.EndUtc   > departureUtc)  // conservative pre-filter
            .Select(b => new
            {
                b.RequiredEmployees,
                b.RequiredBays,
                b.StartUtc,
                b.EndUtc,
                b.TravelTimeMinutes
            })
            .ToListAsync(ct);

        // Precise overlap: a blocker actually conflicts only if its effective window intersects ours
        var blockingBookings = candidateBlockers
            .Where(b =>
            {
                var bDeparture = b.StartUtc.AddMinutes(-(b.TravelTimeMinutes ?? 0));
                var bReturn    = b.EndUtc.AddMinutes(b.TravelTimeMinutes ?? 0);
                return bDeparture < returnUtc && bReturn > departureUtc;
            })
            .ToList();

        var usedEmployees = blockingBookings.Sum(b => b.RequiredEmployees);
        if (availableEmployees - usedEmployees < requiredEmployees)
            throw new DetaillyBusinessRuleException("BOOKING_NO_CAPACITY",
                "Not enough employees available for the selected time.");

        if (request.ServiceMode == ServiceMode.InShop)
        {
            var usedBays = blockingBookings.Sum(b => b.RequiredBays);
            if (location.TotalBays - usedBays < requiredBays)
                throw new DetaillyBusinessRuleException("BOOKING_NO_BAYS",
                    "No bays available for the selected time.");
        }

        var booking = new BookingEntity
        {
            TotalPrice           = totalPrice,
            MobileSurchargeFee   = quote.MobileSurchargeFee > 0 ? quote.MobileSurchargeFee : null,
            FleetDiscountPercent = quote.FleetDiscountPercent > 0 ? quote.FleetDiscountPercent : null,
            StartUtc             = request.StartUtc,
            EndUtc               = endUtc,
            RequiredEmployees    = requiredEmployees,
            RequiredBays         = requiredBays,
            TravelTimeMinutes    = travelTimeMinutes > 0 ? travelTimeMinutes : null,
            ReservationExpiresAtUtc = now.Add(HoldDuration),
            Status               = BookingStatus.PendingPayment,
            Notes                = request.Notes?.Trim(),
            ServiceMode          = request.ServiceMode,
            CustomerId           = customerId,
            ShopLocationId       = request.ShopLocationId,
            ServiceAddressId     = request.ServiceAddressId,
            ServicePackageId     = request.ServicePackageId,
            CreatedAtUtc         = now
        };

        context.Bookings.Add(booking);

        // Fix 4: set both BookingId = 0 (satisfies C# required) and Booking = booking (navigation
        // reference). EF Core's relationship fixup overwrites BookingId with the real generated PK
        // when the INSERT statements are ordered and executed — no intermediate SaveChangesAsync needed.
        if (quote.Addons.Count > 0)
        {
            var bookingItems = quote.Addons.Select(a => new BookingItemEntity
            {
                BookingId                 = 0,      // placeholder; EF fixes up from Booking nav prop
                Booking                   = booking,
                ServicePackageItemId      = a.ServicePackageItemId,
                PriceSnapshot             = a.Price,
                DurationMinutesSnapshot   = a.DurationMinutes,
                RequiredEmployeesSnapshot = a.RequiredEmployees,
                IsAddon                   = true,
                CreatedAtUtc              = now
            });
            context.BookingItems.AddRange(bookingItems);
        }

        if (vehicleIds.Count > 0)
        {
            var assignments = vehicleIds.Select(vid => new BookingVehicleAssignmentEntity
            {
                BookingId    = 0,          // placeholder; EF fixes up from Booking nav prop
                Booking      = booking,
                VehicleId    = vid,
                CreatedAtUtc = now
            });
            context.BookingVehicleAssignments.AddRange(assignments);
        }

        // Fix 4: single save — EF Core's change tracker resolves all FKs from the navigation references
        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return booking.Id;
    }
}
