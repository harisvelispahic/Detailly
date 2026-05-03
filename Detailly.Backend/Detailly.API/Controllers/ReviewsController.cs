using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
using Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
using Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
using Detailly.Application.Modules.Booking.Reviews.Queries.GetMyReview;
using Detailly.Application.Modules.Booking.Reviews.Queries.List;
using Detailly.Application.Modules.Booking.Reviews.Queries.ListMy;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewsController(ISender sender) : ControllerBase
{
    // Create or update a review for a booking (upsert keyed on servicePackageId)
    [HttpPost("{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> CreateOrUpdate(
        int bookingId, CreateReviewCommand command, CancellationToken ct)
    {
        command.BookingId = bookingId;
        var id = await sender.Send(command, ct);
        return Ok(new { id });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<GetReviewByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetReviewByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListReviewsQueryDto>> List([FromQuery] ListReviewsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpGet("my")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<PageResult<ListMyReviewsQueryDto>> ListMy([FromQuery] ListMyReviewsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    // Current user's review for a specific service package (for pre-filling the dialog)
    [HttpGet("my/service-package/{servicePackageId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<GetMyReviewForServicePackageDto?> GetMyReview(int servicePackageId, CancellationToken ct)
    {
        return await sender.Send(
            new GetMyReviewForServicePackageQuery { ServicePackageId = servicePackageId }, ct);
    }
}
