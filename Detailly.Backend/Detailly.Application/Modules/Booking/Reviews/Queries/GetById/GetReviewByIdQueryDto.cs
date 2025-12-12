
namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
public class GetReviewByIdQueryDto
{
    public required int BookingId { get; init; }
    public required int Rating { get; init; }
    public required string? Description { get; init; }
    public required int? ValueForMoney { get; init; }

    public required List<GetReviewByIdQueryDtoImage> Images { get; init; }
}

public class GetReviewByIdQueryDtoImage
{
    public required int Id { get; set; }
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }
}
