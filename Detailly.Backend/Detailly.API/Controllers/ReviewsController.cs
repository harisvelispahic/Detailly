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
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> Create(CreateReviewCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Update(int id, UpdateReviewCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.BookingId = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand { BookingId = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<GetReviewByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetReviewByIdQuery { Id = id }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<ListReviewsQueryDto>> List([FromQuery] ListReviewsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}