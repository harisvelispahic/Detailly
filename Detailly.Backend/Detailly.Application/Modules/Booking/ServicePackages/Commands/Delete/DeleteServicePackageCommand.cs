namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;

public class DeleteServicePackageCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
