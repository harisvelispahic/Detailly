namespace Detailly.Application.Modules.Booking.Reviews.Queries.ListMy;

public class ListMyReviewsQueryDto
{
    public required int Id { get; init; }
    public required int ServicePackageId { get; init; }
    public required string ServicePackageName { get; init; }
    public required int Rating { get; init; }
    public string? Description { get; init; }
    public required List<string> AddonNames { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public DateTime RatedAtUtc => ModifiedAtUtc ?? CreatedAtUtc;
}
