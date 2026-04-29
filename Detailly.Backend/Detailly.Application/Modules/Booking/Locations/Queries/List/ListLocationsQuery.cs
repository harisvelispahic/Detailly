namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQuery : BasePagedQuery<ListLocationsQueryDto>
{
    public string? Search { get; init; }
}
