namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.DownloadImage;

public class DownloadServicePackageImageQueryHandler(IAppDbContext context, ICloudinaryService cloudinaryService)
    : IRequestHandler<DownloadServicePackageImageQuery, DownloadServicePackageImageQueryResult>
{
    public async Task<DownloadServicePackageImageQueryResult> Handle(
        DownloadServicePackageImageQuery request, CancellationToken ct)
    {
        var image = await context.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == request.ImageId && i.ServicePackageId == request.ServicePackageId, ct);

        if (image is null)
            throw new DetaillyNotFoundException("Image not found.");

        var ext = Path.GetExtension(image.ImageUrl).TrimStart('.');
        if (string.IsNullOrWhiteSpace(ext)) ext = "jpg";
        var fileName = $"service-package-{request.ServicePackageId}-image-{request.ImageId}.{ext}";

        var download = await cloudinaryService.DownloadAsync(image.ImageUrl, ct);

        return new DownloadServicePackageImageQueryResult(download.Bytes, download.ContentType, fileName);
    }
}
