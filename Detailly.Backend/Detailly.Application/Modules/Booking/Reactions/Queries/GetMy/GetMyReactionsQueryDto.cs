using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;

public class GetMyReactionsQueryDto
{
    public required int ServicePackageId { get; init; }
    public required ReactionType ReactionType { get; init; }
}
