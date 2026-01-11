namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;

public class DeleteServicePackageCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
