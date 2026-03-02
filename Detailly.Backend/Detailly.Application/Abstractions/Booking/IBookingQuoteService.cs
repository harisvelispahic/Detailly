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
        CancellationToken ct);
}