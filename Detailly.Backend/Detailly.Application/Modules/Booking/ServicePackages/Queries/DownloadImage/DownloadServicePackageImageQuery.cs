namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.DownloadImage;

public class DownloadServicePackageImageQuery : IRequest<DownloadServicePackageImageQueryResult>
{
    [JsonIgnore]
    public int ServicePackageId { get; set; }

    [JsonIgnore]
    public int ImageId { get; set; }
}

public record DownloadServicePackageImageQueryResult(byte[] Bytes, string ContentType, string FileName);
