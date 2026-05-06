namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Delete;

public class DeleteServicePackageImageCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int ServicePackageId { get; set; }

    [JsonIgnore]
    public int ImageId { get; set; }
}
