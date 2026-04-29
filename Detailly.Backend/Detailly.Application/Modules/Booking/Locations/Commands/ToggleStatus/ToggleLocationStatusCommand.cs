namespace Detailly.Application.Modules.Booking.Locations.Commands.ToggleStatus;

public sealed class ToggleLocationStatusCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
