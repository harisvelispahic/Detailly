namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetUploadParams;

public class GetServicePackageUploadParamsQuery : IRequest<GetServicePackageUploadParamsQueryDto>
{
    public int ServicePackageId { get; set; }
}
