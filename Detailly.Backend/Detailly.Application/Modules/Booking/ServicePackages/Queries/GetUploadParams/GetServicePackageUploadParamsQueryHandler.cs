using Detailly.Application.Abstractions;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetUploadParams;

public class GetServicePackageUploadParamsQueryHandler(
    IAppDbContext context,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<GetServicePackageUploadParamsQuery, GetServicePackageUploadParamsQueryDto>
{
    public async Task<GetServicePackageUploadParamsQueryDto> Handle(
        GetServicePackageUploadParamsQuery request, CancellationToken ct)
    {
        var exists = await context.ServicePackages
            .AnyAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (!exists)
            throw new DetaillyNotFoundException("Service package not found.");

        var folder = $"detailly/service-packages/{request.ServicePackageId}";
        var p = cloudinaryService.GenerateDirectUploadParams(folder);

        return new GetServicePackageUploadParamsQueryDto(p.CloudName, p.ApiKey, p.Timestamp, p.Signature, p.Folder);
    }
}
