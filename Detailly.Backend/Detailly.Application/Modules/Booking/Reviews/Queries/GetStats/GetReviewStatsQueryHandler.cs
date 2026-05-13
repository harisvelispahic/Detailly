namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetStats;

public sealed class GetReviewStatsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetReviewStatsQuery, GetReviewStatsQueryDto>
{
    public async Task<GetReviewStatsQueryDto> Handle(GetReviewStatsQuery request, CancellationToken ct)
    {
        var reviews = ctx.Reviews.AsNoTracking().Where(r => !r.IsDeleted);

        var totalCount = await reviews.CountAsync(ct);

        if (totalCount == 0)
            return new GetReviewStatsQueryDto { AverageRating = 0, TotalCount = 0 };

        var averageRating = await reviews.AverageAsync(r => (decimal)r.Rating, ct);

        var distribution = await reviews
            .GroupBy(r => r.Rating)
            .Select(g => new RatingDistributionDto { Stars = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return new GetReviewStatsQueryDto
        {
            AverageRating = Math.Round(averageRating, 1),
            TotalCount = totalCount,
            Distribution = distribution,
        };
    }
}
