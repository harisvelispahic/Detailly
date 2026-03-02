
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Queries.GetById;

public sealed class GetLocationByIdQueryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required LocationType LocationType { get; set; }
    public required int TotalBays { get; set; }

    public required int AddressId { get; set; }
    public required AddressDto Address { get; set; }

    public sealed class AddressDto
    {
        public required int Id { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}