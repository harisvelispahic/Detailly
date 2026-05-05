using Detailly.Application.Modules.Booking.Reactions.Shared;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Reactions.Commands.Upsert;

public class UpsertReactionCommand : IRequest<ReactionSummaryDto>
{
    public int ServicePackageId { get; set; }
    public ReactionType ReactionType { get; init; }
}
