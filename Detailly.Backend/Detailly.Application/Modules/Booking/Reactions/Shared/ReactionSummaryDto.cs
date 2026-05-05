using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Reactions.Shared;

public class ReactionSummaryDto
{
    public required int LikeCount { get; init; }
    public required int DislikeCount { get; init; }
    public ReactionType? MyReaction { get; init; }
}
