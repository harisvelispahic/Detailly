
namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQueryDto
{
    public required int BookingId { get; init; }
    public required int Rating { get; init; }
    public required string? Description { get; init; }
    public required int? ValueForMoney { get; init; }

    public required List<ListReviewsQueryDtoImage> Images { get; init; }
}

public class ListReviewsQueryDtoImage
{
    public required int Id { get; set; }
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }
}
