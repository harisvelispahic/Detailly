using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Booking;
using Detailly.Application.Common.Exceptions;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure.Booking;

public sealed class BookingQuoteService(
    IAppDbContext context,
    IRoadDistanceService roadDistanceService,
    IOptions<OpenRouteServiceOptions> pricingOptions,
    ILogger<BookingQuoteService> logger) : IBookingQuoteService
{
    public async Task<BookingQuoteResult> CalculateAsync(
        int servicePackageId,
        List<int>? addonItemIds,
        ServiceMode serviceMode,
        List<int>? vehicleIds,
        int? customerId,
        bool isFleet,
        CancellationToken ct,
        int? serviceAddressId = null,
        int? shopLocationId = null)
    {
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == servicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        if (isFleet && serviceMode == ServiceMode.InShop)
            throw new DetaillyBusinessRuleException("FLEET_INSHOP_NOT_ALLOWED",
                "Fleet customers can only book mobile (on-address) services.");

        var baseDuration = package.BaseDurationMinutes ?? 0;
        var baseEmployees = package.BaseRequiredEmployees ?? 1;

        // -------------------------
        // Add-on validation + totals
        // -------------------------
        var addonIds = (addonItemIds ?? new List<int>())
            .Distinct()
            .ToList();

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

        // Per-vehicle duration is always base + addons regardless of how many vehicles
        var perVehicleDurationMinutes = baseDuration + addonsDuration;
        if (perVehicleDurationMinutes <= 0)
            throw new DetaillyBusinessRuleException("BOOKING_DURATION_INVALID", "Package duration is invalid.");

        // -------------------------
        // Vehicle multiplier
        // -------------------------
        var distinctVehicleIds = (vehicleIds ?? new List<int>())
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        decimal vehicleMultiplier = 1.0m;

        if (distinctVehicleIds.Count > 0)
        {
            if (customerId is null)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_CONTEXT_MISSING",
                    "Customer context is required when vehicles are provided.");

            if (!isFleet && distinctVehicleIds.Count != 1)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_COUNT_INVALID",
                    "Non-fleet customers must select exactly one vehicle.");

            var vehicles = await context.Vehicles
                .AsNoTracking()
                .Where(v => distinctVehicleIds.Contains(v.Id) && !v.IsDeleted && v.ApplicationUserId == customerId.Value)
                .Select(v => new { v.Id, v.VehicleCategoryId })
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

            vehicleMultiplier = vehicles
                .Select(v => categoryMultiplierById[v.VehicleCategoryId])
                .DefaultIfEmpty(1.0m)
                .Max();

            if (vehicleMultiplier <= 0)
                vehicleMultiplier = 1.0m;
        }

        var totalPrice = (package.Price + addonsPrice) * vehicleMultiplier;

        // -------------------------
        // Capacity figures
        //
        // InShop fleet: vehicles are serviced in parallel (one bay + employee group each).
        // Mobile / non-fleet: the handler does k-optimisation for fleet; single vehicle otherwise.
        // -------------------------
        int requiredEmployees;
        int requiredBays;
        int totalDurationMinutes;

        var numVehicles = Math.Max(1, distinctVehicleIds.Count);

        if (serviceMode == ServiceMode.InShop && isFleet && numVehicles > 1)
        {
            // Parallel bays: each vehicle needs its own bay and employee group simultaneously.
            var baseRequiredPerVehicle = Math.Max(baseEmployees, addonsEmployeesMax);
            requiredEmployees = baseRequiredPerVehicle * numVehicles;
            requiredBays = numVehicles;
            totalDurationMinutes = perVehicleDurationMinutes; // all work happens in parallel
        }
        else
        {
            requiredEmployees = Math.Max(baseEmployees, addonsEmployeesMax);
            requiredBays = serviceMode == ServiceMode.InShop ? 1 : 0;
            totalDurationMinutes = perVehicleDurationMinutes;
        }

        // -------------------------
        // Mobile surcharge + travel time (road distance via ORS)
        // -------------------------
        decimal mobileSurchargeFee = 0m;
        int travelTimeMinutes = 0;

        if (serviceMode == ServiceMode.Mobile && serviceAddressId is not null && shopLocationId is not null)
        {
            var travel = await CalculateMobileTravelAsync(
                serviceAddressId.Value,
                shopLocationId.Value,
                ct);

            mobileSurchargeFee = travel.SurchargeFee;
            travelTimeMinutes = travel.TravelTimeMinutes;
            totalPrice += mobileSurchargeFee;
        }

        return new BookingQuoteResult
        {
            ServicePackageId = servicePackageId,
            TotalDurationMinutes = totalDurationMinutes,
            PerVehicleDurationMinutes = perVehicleDurationMinutes,
            RequiredEmployees = requiredEmployees,
            RequiredBays = requiredBays,
            TotalPrice = totalPrice,
            MobileSurchargeFee = mobileSurchargeFee,
            TravelTimeMinutes = travelTimeMinutes,
            Addons = addons.Select(a => new BookingQuoteResult.AddonSnapshot
            {
                ServicePackageItemId = a.Id,
                Price = a.Price,
                DurationMinutes = a.DurationMinutes,
                RequiredEmployees = a.RequiredEmployees
            }).ToList()
        };
    }

    private record MobileTravelResult(decimal SurchargeFee, int TravelTimeMinutes);

    private async Task<MobileTravelResult> CalculateMobileTravelAsync(
        int serviceAddressId,
        int shopLocationId,
        CancellationToken ct)
    {
        var opts = pricingOptions.Value;

        var serviceAddress = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == serviceAddressId && !a.IsDeleted, ct);

        if (serviceAddress is null)
            return ApplyFallback(opts, "Service address not found.");

        if (serviceAddress.Latitude is null || serviceAddress.Longitude is null)
            return ApplyFallback(opts, $"Address {serviceAddressId} has no coordinates — cannot calculate road distance.");

        var shopAddress = await context.Locations
            .AsNoTracking()
            .Where(l => l.Id == shopLocationId && !l.IsDeleted)
            .Select(l => new { l.Address.Latitude, l.Address.Longitude })
            .FirstOrDefaultAsync(ct);

        if (shopAddress is null || shopAddress.Latitude is null || shopAddress.Longitude is null)
            return ApplyFallback(opts, $"Shop location {shopLocationId} has no coordinates — cannot calculate road distance.");

        decimal distanceKm;
        int travelTimeMinutes;

        // Use cached values when they were computed for the same shop
        if (serviceAddress.DistanceFromShopKm is not null &&
            serviceAddress.TravelTimeFromShopMinutes is not null &&
            serviceAddress.TravelMetadataLocationId == shopLocationId)
        {
            distanceKm = serviceAddress.DistanceFromShopKm.Value;
            travelTimeMinutes = serviceAddress.TravelTimeFromShopMinutes.Value;
        }
        else
        {
            var apiResult = await roadDistanceService.GetRoadTravelAsync(
                shopAddress.Latitude.Value, shopAddress.Longitude.Value,
                serviceAddress.Latitude.Value, serviceAddress.Longitude.Value,
                ct);

            if (apiResult is null)
                return ApplyFallback(opts, "Road travel API returned no result.");

            distanceKm = apiResult.DistanceKm;
            travelTimeMinutes = apiResult.TravelTimeMinutes;

            // Persist cache (best-effort — don't fail the quote if this save fails)
            try
            {
                serviceAddress.DistanceFromShopKm = distanceKm;
                serviceAddress.TravelTimeFromShopMinutes = travelTimeMinutes;
                serviceAddress.TravelMetadataLocationId = shopLocationId;
                await context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to cache travel info for address {AddressId}.", serviceAddressId);
            }
        }

        return new MobileTravelResult(ComputeSurcharge(distanceKm, opts), travelTimeMinutes);
    }

    private MobileTravelResult ApplyFallback(OpenRouteServiceOptions opts, string reason)
    {
        logger.LogWarning("Mobile travel fallback applied ({Reason}). Using fixed fee {Fee}, travel time 0.", reason, opts.FallbackSurcharge);
        return new MobileTravelResult(opts.FallbackSurcharge, 0);
    }

    private static decimal ComputeSurcharge(decimal distanceKm, OpenRouteServiceOptions opts)
    {
        if (distanceKm <= opts.FreeRadiusKm)
            return 0m;

        var billableKm = distanceKm - opts.FreeRadiusKm;
        return billableKm * opts.PricePerKm;
    }
}
