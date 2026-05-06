namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetImage;

public class GetServicePackageImageQueryHandler(IAppDbContext context)
    : IRequestHandler<GetServicePackageImageQuery, GetServicePackageImageQueryResult>
{
    public async Task<GetServicePackageImageQueryResult> Handle(
        GetServicePackageImageQuery request, CancellationToken ct)
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

        return new GetServicePackageImageQueryResult(image.ImageUrl, fileName);
    }
}
