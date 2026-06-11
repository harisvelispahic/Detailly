using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Abstractions.Booking;

public interface IBookingQuoteService
{
    Task<BookingQuoteResult> CalculateAsync(
        int servicePackageId,
        List<int>? addonItemIds,
        ServiceMode serviceMode,
        List<int>? vehicleIds,
        int? customerId,
        bool isFleet,
        CancellationToken ct,
        int? serviceAddressId = null,
        int? shopLocationId = null);

    /// <summary>
    /// Pure k-optimisation for fleet mobile multi-vehicle slots.
    /// Returns null when the slot cannot accommodate any valid schedule.
    /// </summary>
    FleetMobileCapacityResult? ComputeFleetMobileCapacity(
        int vehicleCount,
        int baseEmployeesPerVehicle,
        int perVehicleDurationMinutes,
        DateTime slotStartUtc,
        int travelTimeMinutes,
        DateTime maxShiftEnd);
}

public sealed record FleetMobileCapacityResult(int RequiredEmployees, int TotalDurationMinutes);