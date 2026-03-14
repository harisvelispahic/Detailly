namespace Detailly.Application.Modules.Booking.Locations.Queries.GetById;

public sealed class GetLocationByIdQuery : IRequest<GetLocationByIdQueryDto>
{
    public required int Id { get; set; }
}