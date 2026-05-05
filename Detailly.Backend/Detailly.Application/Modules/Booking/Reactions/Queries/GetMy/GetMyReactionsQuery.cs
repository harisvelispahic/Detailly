using Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;

namespace Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;

public record GetMyReactionsQuery : IRequest<List<MyReactionDto>>;
