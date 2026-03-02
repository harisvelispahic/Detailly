
namespace Detailly.Application.Modules.Booking.Locations.Commands.Delete;

public sealed class DeleteLocationCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}