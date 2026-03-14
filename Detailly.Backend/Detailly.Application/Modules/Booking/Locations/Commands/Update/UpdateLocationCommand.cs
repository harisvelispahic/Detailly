using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Update;

public sealed class UpdateLocationCommand : IRequest<Unit>
{
    public required int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public LocationType? LocationType { get; set; }
    public int? TotalBays { get; set; } // only meaningful for Shop

    // Optional: allow changing address by id
    public int? AddressId { get; set; }
}