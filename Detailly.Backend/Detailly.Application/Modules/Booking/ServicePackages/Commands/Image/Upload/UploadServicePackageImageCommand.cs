using Detailly.Application.Modules.Booking.ServicePackages.Shared;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Upload;

public class UploadServicePackageImageCommand : IRequest<ServicePackageImageDto>
{
    public int ServicePackageId { get; set; }
    public required Stream FileStream { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
}
