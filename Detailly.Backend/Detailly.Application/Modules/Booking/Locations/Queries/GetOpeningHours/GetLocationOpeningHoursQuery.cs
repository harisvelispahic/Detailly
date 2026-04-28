namespace Detailly.Application.Modules.Booking.Locations.Queries.GetOpeningHours;

public sealed class GetLocationOpeningHoursQuery : IRequest<List<GetLocationOpeningHoursQueryDto>>
{
    public required int LocationId { get; init; }
}
