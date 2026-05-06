namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.SetThumbnail;

public class SetServicePackageThumbnailCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int ServicePackageId { get; set; }

    [JsonIgnore]
    public int ImageId { get; set; }
}
