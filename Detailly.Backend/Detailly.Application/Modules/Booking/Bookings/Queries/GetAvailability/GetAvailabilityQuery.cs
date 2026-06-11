using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQuery : IRequest<GetAvailabilityResult>
{
    public required DateTime DateUtc { get; set; } // date part is used
    public required int ServicePackageId { get; set; }
    public List<int>? AddonItemIds { get; set; }

    public required ServiceMode ServiceMode { get; set; }

    // since you have one shop, keep it explicit anyway
    public required int ShopLocationId { get; set; }

    // For mobile bookings: used to calculate real travel time and filter impossible slots
    public int? ServiceAddressId { get; set; }

    // Required for fleet customers: drives multi-vehicle capacity and k-optimisation
    public List<int>? VehicleIds { get; set; }
}