
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Create;
public class CreateReviewCommand : IRequest<int>
{
    public required int BookingId { get; set; }
    public required int Rating { get; set; }
    public string? Description { get; set; } = string.Empty;
    public int? ValueForMoney { get; set; }

    public List<CreateReviewCommandImage>? Images { get; set; }
}

public class CreateReviewCommandImage
{
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }
}
