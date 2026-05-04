using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;

public sealed class ExportMyBookingsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<ExportMyBookingsQuery, List<ExportMyBookingsQueryDto>>
{
    public async Task<List<ExportMyBookingsQueryDto>> Handle(ExportMyBookingsQuery request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var start = request.StartDateUtc.Date;
        var end = request.EndDateUtc.Date.AddDays(1);

        return await context.Bookings
            .AsNoTracking()
            .Where(b =>
                !b.IsDeleted &&
                b.CustomerId == currentUser.ApplicationUserId.Value &&
                b.StartUtc >= start &&
                b.StartUtc < end &&
                b.Status != BookingStatus.Draft)
            .OrderBy(b => b.StartUtc)
            .Select(b => new ExportMyBookingsQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                TotalPrice = b.TotalPrice,
                ServicePackageName = b.ServicePackage.Name,
            })
            .ToListAsync(ct);
    }
}
