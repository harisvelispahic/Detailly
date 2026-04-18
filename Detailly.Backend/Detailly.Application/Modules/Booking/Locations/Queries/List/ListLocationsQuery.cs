using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQuery : BasePagedQuery<ListLocationsQueryDto>
{
    public string? Search { get; init; }
    public LocationType? LocationType { get; set; } // optional filter
}