using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
using Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
using Detailly.Application.Modules.Booking.Reviews.Commands.Update;
using Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
using Detailly.Application.Modules.Booking.Reviews.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewsController(ISender sender) : ControllerBase
{
    [HttpPost("{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> Create(int bookingId, CreateReviewCommand command, CancellationToken ct)
    {
        command.BookingId = bookingId;
        await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { bookingId }, new { bookingId });
    }

    [HttpPut("{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Update(int bookingId, UpdateReviewCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.BookingId = bookingId;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Delete(int bookingId, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand { BookingId = bookingId }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{bookingId:int}")]
    [AllowAnonymous]
    public async Task<GetReviewByIdQueryDto> GetById(int bookingId, CancellationToken ct)
    {
        var category = await sender.Send(new GetReviewByIdQuery { BookingId = bookingId }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListReviewsQueryDto>> List([FromQuery] ListReviewsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}