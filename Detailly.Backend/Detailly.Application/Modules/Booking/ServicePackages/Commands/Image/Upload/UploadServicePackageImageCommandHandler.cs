using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Booking.ServicePackages.Shared;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Upload;

public class UploadServicePackageImageCommandHandler(
    IAppDbContext context,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<UploadServicePackageImageCommand, ServicePackageImageDto>
{
    public async Task<ServicePackageImageDto> Handle(UploadServicePackageImageCommand request, CancellationToken ct)
    {
        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        var hasImages = await context.Images
            .AnyAsync(i => i.ServicePackageId == request.ServicePackageId, ct);

        var maxOrder = await context.Images
            .Where(i => i.ServicePackageId == request.ServicePackageId)
            .MaxAsync(i => (int?)i.DisplayOrder, ct) ?? -1;

        var uploadResult = await cloudinaryService.UploadAsync(request.FileStream, request.FileName, ct);

        var image = new ImageEntity
        {
            ImageUrl = uploadResult.SecureUrl,
            PublicId = uploadResult.PublicId,
            ServicePackageId = request.ServicePackageId,
            IsThumbnail = !hasImages,   // first image becomes thumbnail automatically
            DisplayOrder = maxOrder + 1,
        };

        context.Images.Add(image);
        await context.SaveChangesAsync(ct);

        return new ServicePackageImageDto(
            image.Id, image.ImageUrl, image.PublicId, image.IsThumbnail, image.DisplayOrder);
    }
}
