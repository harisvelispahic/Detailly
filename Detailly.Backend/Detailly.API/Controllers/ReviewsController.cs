
using Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
using Detailly.Application.Modules.Booking.Reviews.Commands.Update;
using Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
using Detailly.Application.Modules.Booking.Reviews.Queries.List;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ReviewsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<int>> CreateReview(CreateReviewCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateReviewCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.BookingId = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand { BookingId = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetReviewByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetReviewByIdQuery { Id = id }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<List<ListReviewsQueryDto>> List([FromQuery] ListReviewsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}