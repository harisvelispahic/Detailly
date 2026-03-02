
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Create;

public sealed class CreateLocationCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public required LocationType LocationType { get; set; }

    // Required when LocationType = Shop (otherwise ignored / forced to 0)
    public int? TotalBays { get; set; }

    // Either use an existing AddressId OR provide a new address payload
    public int? AddressId { get; set; }
    public CreateLocationAddressDto? Address { get; set; }
}

public sealed class CreateLocationAddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}