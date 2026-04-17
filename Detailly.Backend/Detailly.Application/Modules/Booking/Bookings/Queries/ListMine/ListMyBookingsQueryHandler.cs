namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMine;

public sealed class ListMyBookingsQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListMyBookingsQuery, PageResult<ListMyBookingsQueryDto>>
{
    public async Task<PageResult<ListMyBookingsQueryDto>> Handle(ListMyBookingsQuery request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var q = context.Bookings.AsNoTracking();

        var projectedQuery = q.OrderByDescending(b => b.StartUtc)
            .Where(b => !b.IsDeleted && b.CustomerId == appCurrentUser.ApplicationUserId.Value)
            .Select(b => new ListMyBookingsQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                TotalPrice = b.TotalPrice,
                ServicePackageName = b.ServicePackage.Name
            });

        return await PageResult<ListMyBookingsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);

    }
}
