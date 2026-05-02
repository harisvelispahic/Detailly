namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;

public class GetReviewByIdQueryDto
{
    public required int Id { get; init; }
    public required int BookingId { get; init; }
    public required int ServicePackageId { get; init; }
    public required int Rating { get; init; }
    public required string? Description { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
