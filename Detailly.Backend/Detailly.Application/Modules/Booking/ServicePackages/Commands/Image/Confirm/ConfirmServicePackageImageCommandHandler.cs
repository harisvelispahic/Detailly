using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Booking.ServicePackages.Shared;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Confirm;

public class ConfirmServicePackageImageCommandHandler(
    IAppDbContext context,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<ConfirmServicePackageImageCommand, ServicePackageImageDto>
{
    public async Task<ServicePackageImageDto> Handle(ConfirmServicePackageImageCommand request, CancellationToken ct)
    {
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        var isDuplicate = await context.Images
            .AnyAsync(i => i.ServicePackageId == request.ServicePackageId && i.FileHash == request.FileHash, ct);

        if (isDuplicate)
        {
            // Delete the orphaned Cloudinary asset so it doesn't accumulate
            await cloudinaryService.DeleteAsync(request.PublicId, ct);
            throw new DetaillyConflictException("This image has already been uploaded to this package.");
        }

        var hasImages = await context.Images
            .AnyAsync(i => i.ServicePackageId == request.ServicePackageId, ct);

        var maxOrder = await context.Images
            .Where(i => i.ServicePackageId == request.ServicePackageId)
            .MaxAsync(i => (int?)i.DisplayOrder, ct) ?? -1;

        var image = new ImageEntity
        {
            ImageUrl = request.SecureUrl,
            PublicId = request.PublicId,
            FileHash = request.FileHash,
            ServicePackageId = request.ServicePackageId,
            IsThumbnail = !hasImages,
            DisplayOrder = maxOrder + 1,
        };

        context.Images.Add(image);
        await context.SaveChangesAsync(ct);

        return new ServicePackageImageDto(
            image.Id, image.ImageUrl, image.PublicId, image.IsThumbnail, image.DisplayOrder);
    }
}
