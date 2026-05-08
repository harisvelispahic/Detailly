using Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;
using Detailly.Domain.Common.Enums;

public class GetMyReactionsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyReactionsQuery, List<GetMyReactionsQueryDto>>
{
    public async Task<List<GetMyReactionsQueryDto>> Handle(GetMyReactionsQuery request, CancellationToken ct)
    {
        var userId = currentUser.ApplicationUserId!.Value;

        return await context.Reactions
            .AsNoTracking()
            .Where(r => r.CustomerId == userId && r.ReactionType != ReactionType.None)
            .Select(r => new GetMyReactionsQueryDto
            {
                ServicePackageId = r.ServicePackageId,
                ReactionType = r.ReactionType,
            })
            .ToListAsync(ct);
    }
}
