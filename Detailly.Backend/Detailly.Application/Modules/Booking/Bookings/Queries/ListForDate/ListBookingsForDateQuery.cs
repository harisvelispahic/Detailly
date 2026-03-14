using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;

public sealed record ListBookingsForDateQuery(
    DateTime DateUtc,
    int ShopLocationId,
    ServiceMode ServiceMode,
    bool IncludePendingPaymentHolds = false
) : IRequest<List<ListBookingsForDateQueryDto>>;