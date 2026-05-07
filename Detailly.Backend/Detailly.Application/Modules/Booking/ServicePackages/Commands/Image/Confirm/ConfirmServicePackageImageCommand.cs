using Detailly.Application.Modules.Booking.ServicePackages.Shared;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Confirm;

public class ConfirmServicePackageImageCommand : IRequest<ServicePackageImageDto>
{
    public int ServicePackageId { get; set; }
    public required string PublicId { get; set; }
    public required string SecureUrl { get; set; }
    public required string FileHash { get; set; }
}
