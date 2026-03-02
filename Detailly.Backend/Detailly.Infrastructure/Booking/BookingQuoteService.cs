using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Booking;
using Detailly.Application.Common.Exceptions;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Booking;

public sealed class BookingQuoteService(IAppDbContext context) : IBookingQuoteService
{
    public async Task<BookingQuoteResult> CalculateAsync(
        int servicePackageId,
        List<int>? addonItemIds,
        ServiceMode serviceMode,
        List<int>? vehicleIds,
        int? customerId,
        bool isFleet,
        CancellationToken ct)
    {
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == servicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        var baseDuration = package.BaseDurationMinutes ?? 0;
        var baseEmployees = package.BaseRequiredEmployees ?? 1;

        // -------------------------
        // Add-on validation + totals
        // -------------------------
        var addonIds = (addonItemIds ?? new List<int>())
            .Distinct()
            .ToList();

        // Prevent selecting add-ons already in base package
        if (addonIds.Count > 0)
        {
            var baseItemIds = await context.ServicePackageItemAssignments
                .Where(a => a.ServicePackageId == servicePackageId && !a.IsDeleted)
                .Select(a => a.ServicePackageItemId)
                .ToListAsync(ct);

            if (addonIds.Intersect(baseItemIds).Any())
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_DUPLICATE",
                    "One or more add-ons are already included in the selected package.");
        }

        List<ServicePackageItemEntity> addons = new();
        if (addonIds.Count > 0)
        {
            addons = await context.ServicePackageItems
                .Where(i => addonIds.Contains(i.Id) && !i.IsDeleted && i.IsAddon && i.IsActive)
                .ToListAsync(ct);

            if (addons.Count != addonIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_INVALID",
                    "One or more add-on items are invalid or inactive.");
        }

        var addonsDuration = addons.Sum(x => x.DurationMinutes);
        var addonsEmployeesMax = addons.Count == 0 ? 0 : addons.Max(x => x.RequiredEmployees);
        var addonsPrice = addons.Sum(x => x.Price);

        var totalDurationMinutes = baseDuration + addonsDuration;
        if (totalDurationMinutes <= 0)
            throw new DetaillyBusinessRuleException("BOOKING_DURATION_INVALID", "Package duration is invalid.");

        var requiredEmployees = Math.Max(baseEmployees, addonsEmployeesMax);
        var requiredBays = serviceMode == ServiceMode.InShop ? 1 : 0;

        // -------------------------
        // Vehicle multiplier (Option A: price-only)
        // -------------------------
        // Rules:
        // - If vehicleIds provided:
        //   - Non-fleet => must be exactly 1
        //   - Fleet => can be 1+
        //   - Must be owned by customer (if customerId provided)
        // - If no vehicleIds provided (availability flow), multiplier = 1.0
        var distinctVehicleIds = (vehicleIds ?? new List<int>())
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        decimal vehicleMultiplier = 1.0m;

        if (distinctVehicleIds.Count > 0)
        {
            if (customerId is null)
            {
                // If we have vehicles but no customer context, we can’t validate ownership.
                // Treat as invalid usage (prevents abuse).
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_CONTEXT_MISSING",
                    "Customer context is required when vehicles are provided.");
            }

            if (!isFleet && distinctVehicleIds.Count != 1)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_COUNT_INVALID",
                    "Non-fleet customers must select exactly one vehicle.");

            // Load vehicles + categories (must be owned by customer)
            var vehicles = await context.Vehicles
                .AsNoTracking()
                .Where(v => distinctVehicleIds.Contains(v.Id) && !v.IsDeleted && v.ApplicationUserId == customerId.Value)
                .Select(v => new
                {
                    v.Id,
                    v.VehicleCategoryId
                })
                .ToListAsync(ct);

            if (vehicles.Count != distinctVehicleIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_INVALID",
                    "One or more vehicles are invalid or not owned by you.");

            var categoryIds = vehicles.Select(v => v.VehicleCategoryId).Distinct().ToList();

            var categories = await context.VehicleCategories
                .AsNoTracking()
                .Where(c => categoryIds.Contains(c.Id) && !c.IsDeleted)
                .Select(c => new { c.Id, c.BasePriceMultiplier })
                .ToListAsync(ct);

            if (categories.Count != categoryIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_CATEGORY_INVALID",
                    "One or more vehicle categories are missing.");

            var categoryMultiplierById = categories.ToDictionary(x => x.Id, x => x.BasePriceMultiplier);

            // If multiple vehicles (fleet), you have two choices:
            // A) Use max multiplier (simpler)
            // B) Use average multiplier
            // For Option A price-only, I recommend MAX (it’s conservative).
            vehicleMultiplier = vehicles
                .Select(v => categoryMultiplierById[v.VehicleCategoryId])
                .DefaultIfEmpty(1.0m)
                .Max();

            if (vehicleMultiplier <= 0)
                vehicleMultiplier = 1.0m;
        }

        // Base price + addons, then apply multiplier
        var totalPrice = (package.Price + addonsPrice) * vehicleMultiplier;

        return new BookingQuoteResult
        {
            ServicePackageId = servicePackageId,
            TotalDurationMinutes = totalDurationMinutes,
            RequiredEmployees = requiredEmployees,
            RequiredBays = requiredBays,
            TotalPrice = totalPrice,
            Addons = addons.Select(a => new BookingQuoteResult.AddonSnapshot
            {
                ServicePackageItemId = a.Id,
                Price = a.Price,
                DurationMinutes = a.DurationMinutes,
                RequiredEmployees = a.RequiredEmployees
            }).ToList()
        };
    }
}