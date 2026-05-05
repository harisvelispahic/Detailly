using Detailly.Application.Modules.Booking.Reactions.Commands.Upsert;
using Detailly.Application.Modules.Booking.Reactions.Shared;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

public class UpsertReactionCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<UpsertReactionCommand, ReactionSummaryDto>
{
    public async Task<ReactionSummaryDto> Handle(UpsertReactionCommand request, CancellationToken ct)
    {
        var userId = currentUser.ApplicationUserId!.Value;

        var existing = await context.Reactions
            .FirstOrDefaultAsync(r => r.ServicePackageId == request.ServicePackageId
                                      && r.CustomerId == userId, ct);

        if (existing is not null)
        {
            // Same type = toggle off (set to None); different type = switch
            existing.ReactionType = existing.ReactionType == request.ReactionType
                ? ReactionType.None
                : request.ReactionType;
        }
        else
        {
            context.Reactions.Add(new ReactionEntity
            {
                ServicePackageId = request.ServicePackageId,
                CustomerId = userId,
                ReactionType = request.ReactionType,
            });
        }

        await context.SaveChangesAsync(ct);

        var likeCount = await context.Reactions
            .CountAsync(r => r.ServicePackageId == request.ServicePackageId
                             && r.ReactionType == ReactionType.Like, ct);

        var dislikeCount = await context.Reactions
            .CountAsync(r => r.ServicePackageId == request.ServicePackageId
                             && r.ReactionType == ReactionType.Dislike, ct);

        var currentType = await context.Reactions
            .Where(r => r.ServicePackageId == request.ServicePackageId && r.CustomerId == userId)
            .Select(r => (ReactionType?)r.ReactionType)
            .FirstOrDefaultAsync(ct);

        return new ReactionSummaryDto
        {
            LikeCount = likeCount,
            DislikeCount = dislikeCount,
            MyReaction = currentType == ReactionType.None ? null : currentType,
        };
    }
}
