using System.Security.Cryptography;
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

        // Buffer stream so we can hash it before sending to Cloudinary
        using var buffer = new MemoryStream();
        await request.FileStream.CopyToAsync(buffer, ct);
        buffer.Position = 0;

        var hashBytes = await SHA256.HashDataAsync(buffer, ct);
        var fileHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
        buffer.Position = 0;

        var isDuplicate = await context.Images
            .AnyAsync(i => i.ServicePackageId == request.ServicePackageId && i.FileHash == fileHash, ct);

        if (isDuplicate)
            throw new DetaillyConflictException("This image has already been uploaded to this package.");

        var hasImages = await context.Images
            .AnyAsync(i => i.ServicePackageId == request.ServicePackageId, ct);

        var maxOrder = await context.Images
            .Where(i => i.ServicePackageId == request.ServicePackageId)
            .MaxAsync(i => (int?)i.DisplayOrder, ct) ?? -1;

        var uploadResult = await cloudinaryService.UploadAsync(buffer, request.FileName, ct);

        var image = new ImageEntity
        {
            ImageUrl = uploadResult.SecureUrl,
            PublicId = uploadResult.PublicId,
            FileHash = fileHash,
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
