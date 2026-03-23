namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;

public class GetServicePackageByIdQuery : IRequest<GetServicePackageByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}
