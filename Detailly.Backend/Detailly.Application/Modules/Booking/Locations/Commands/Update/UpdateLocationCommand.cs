using Detailly.Application.Modules.Booking.Locations.Commands.Create;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Update;

public sealed class UpdateLocationCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? TotalBays { get; set; }

    public UpdateLocationAddressDto? Address { get; set; }

    public List<LocationOpeningHoursInputDto>? OpeningHours { get; set; }
}

public sealed class UpdateLocationAddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
}
