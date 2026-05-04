namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Delete;

public class DeleteServicePackageItemCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
