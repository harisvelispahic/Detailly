using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMine;

public sealed class ListMyBookingsQueryHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    TimeProvider timeProvider)
    : IRequestHandler<ListMyBookingsQuery, PageResult<ListMyBookingsQueryDto>>
{
    private const int ReviewWindowDays = 7;

    public async Task<PageResult<ListMyBookingsQueryDto>> Handle(ListMyBookingsQuery request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var cutoff = timeProvider.GetUtcNow().UtcDateTime.AddDays(-ReviewWindowDays);

        var projectedQuery = context.Bookings
            .AsNoTracking()
            .Where(b => !b.IsDeleted && b.CustomerId == userId)
            .OrderByDescending(b => b.StartUtc)
            .Select(b => new ListMyBookingsQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                TotalPrice = b.TotalPrice,
                ServicePackageName = b.ServicePackage.Name,
                ServicePackageId = b.ServicePackageId,
                CanRate = b.Status == BookingStatus.Completed && b.EndUtc >= cutoff,
            });

        return await PageResult<ListMyBookingsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
