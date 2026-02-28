
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
        CancellationToken ct)
    {
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == servicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        var baseDuration = package.BaseDurationMinutes ?? 0;
        var baseEmployees = package.BaseRequiredEmployees ?? 1;

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
        var totalPrice = package.Price + addonsPrice;

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