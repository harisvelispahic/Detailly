namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMine;

public sealed class ListMyBookingsQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListMyBookingsQuery, List<ListMyBookingsQueryDto>>
{
    public async Task<List<ListMyBookingsQueryDto>> Handle(ListMyBookingsQuery request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        return await context.Bookings
            .AsNoTracking()
            .Where(b => !b.IsDeleted && b.CustomerId == customerId)
            .OrderByDescending(b => b.StartUtc)
            .Select(b => new ListMyBookingsQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                TotalPrice = b.TotalPrice,
                ServicePackageName = b.ServicePackage.Name
            })
            .ToListAsync(ct);
    }
}