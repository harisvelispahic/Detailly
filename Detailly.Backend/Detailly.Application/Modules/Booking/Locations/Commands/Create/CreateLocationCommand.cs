namespace Detailly.Application.Modules.Booking.Locations.Commands.Create;

public sealed class CreateLocationCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int TotalBays { get; set; }

    public required CreateLocationAddressDto Address { get; set; }

    public List<LocationOpeningHoursInputDto>? OpeningHours { get; set; }
}

public sealed class CreateLocationAddressDto
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public string? Region { get; set; }
    public required string Country { get; set; }
}

public sealed class LocationOpeningHoursInputDto
{
    public required int DayOfWeek { get; set; } // 0=Sunday … 6=Saturday
    public bool IsClosed { get; set; }
    public int? OpenHour { get; set; }    // 0–23 UTC
    public int? OpenMinute { get; set; }  // 0–59
    public int? CloseHour { get; set; }
    public int? CloseMinute { get; set; }
}
