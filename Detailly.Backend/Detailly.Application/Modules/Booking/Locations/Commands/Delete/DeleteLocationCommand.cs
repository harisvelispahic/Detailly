namespace Detailly.Application.Modules.Booking.Locations.Commands.Delete;

public sealed class DeleteLocationCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}