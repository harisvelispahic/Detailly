namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetImage;

public class GetServicePackageImageQuery : IRequest<GetServicePackageImageQueryResult>
{
    [JsonIgnore]
    public int ServicePackageId { get; set; }

    [JsonIgnore]
    public int ImageId { get; set; }
}

public record GetServicePackageImageQueryResult(string ImageUrl, string FileName);
