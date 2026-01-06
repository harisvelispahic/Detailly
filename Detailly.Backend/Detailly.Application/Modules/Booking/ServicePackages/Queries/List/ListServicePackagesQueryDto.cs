namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.List;

public class ListServicePackagesQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required decimal Price { get; init; }
    public required int EstimatedDurationHours { get; init; }

    public required List<ListServicePackagesQueryDtoItem> Items { get; init; }
}

public class ListServicePackagesQueryDtoItem
{
    public required int Id { get; set; } // ServicePackageItemId
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
}
