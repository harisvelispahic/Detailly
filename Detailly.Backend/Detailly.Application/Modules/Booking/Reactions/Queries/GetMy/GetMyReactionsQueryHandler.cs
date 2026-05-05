using Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;
using Detailly.Domain.Common.Enums;

public class GetMyReactionsQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyReactionsQuery, List<MyReactionDto>>
{
    public async Task<List<MyReactionDto>> Handle(GetMyReactionsQuery request, CancellationToken ct)
    {
        var userId = currentUser.ApplicationUserId!.Value;

        return await context.Reactions
            .AsNoTracking()
            .Where(r => r.CustomerId == userId && r.ReactionType != ReactionType.None)
            .Select(r => new MyReactionDto
            {
                ServicePackageId = r.ServicePackageId,
                ReactionType = r.ReactionType,
            })
            .ToListAsync(ct);
    }
}
