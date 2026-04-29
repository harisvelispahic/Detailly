namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int TotalBays { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public bool IsOpenToday { get; set; }
    public bool IsTemporarilyClosed { get; set; }
}
