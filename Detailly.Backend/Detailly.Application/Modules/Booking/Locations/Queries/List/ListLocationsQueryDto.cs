using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required LocationType LocationType { get; set; }
    public required int TotalBays { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}