using Detailly.Application.Modules.Booking.Reactions.Commands.Upsert;
using Detailly.Application.Modules.Booking.Reactions.Queries.GetMy;
using Detailly.Application.Modules.Booking.Reactions.Shared;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReactionsController(ISender sender) : ControllerBase
{
    // Toggle like/dislike for a service package: same type = remove, different type = switch
    [HttpPut("service-packages/{servicePackageId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ReactionSummaryDto> Upsert(
        int servicePackageId,
        UpsertReactionCommand command,
        CancellationToken ct)
    {
        command.ServicePackageId = servicePackageId;
        return await sender.Send(command, ct);
    }

    // Get all reactions by the current user (for pre-filling the UI state)
    [HttpGet("service-packages/my")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<List<MyReactionDto>> GetMy(CancellationToken ct)
    {
        return await sender.Send(new GetMyReactionsQuery(), ct);
    }
}
