namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQueryDto
{
    public required int Id { get; init; }
    public required int ServicePackageId { get; init; }
    public required string ServicePackageName { get; init; }
    public required string CustomerFullName { get; init; }
    public required string CustomerInitials { get; init; }
    public required int Rating { get; init; }
    public required string? Description { get; init; }
    public required List<string> AddonNames { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
