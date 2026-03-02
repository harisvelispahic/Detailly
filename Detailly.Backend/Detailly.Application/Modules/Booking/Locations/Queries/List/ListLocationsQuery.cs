
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQuery : IRequest<List<ListLocationsQueryDto>>
{
    public LocationType? LocationType { get; set; } // optional filter
}