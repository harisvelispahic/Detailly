
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Update;
public class UpdateReviewCommand : IRequest<Unit>
{
    public required int BookingId { get; set; }
    public int? Rating { get; set; }
    public string? Description { get; set; }
    public int? ValueForMoney { get; set; }
    public List<UpdateReviewCommandImage>? Images { get; set; }
}

public class UpdateReviewCommandImage
{
    public string ImageUrl { get; set; } = null!;
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }
}