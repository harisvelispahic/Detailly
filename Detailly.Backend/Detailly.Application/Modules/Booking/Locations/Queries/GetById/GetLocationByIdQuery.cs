namespace Detailly.Application.Modules.Booking.Locations.Queries.GetById;

public sealed class GetLocationByIdQuery : IRequest<GetLocationByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}