namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;

public class ListServicePackageItemsQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    public required int DurationMinutes { get; init; }
    public required int RequiredEmployees { get; init; }
    public required bool IsAddon { get; init; }
    public required bool IsActive { get; init; }
}
