namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;

public class GetServicePackageByIdQuery : IRequest<GetServicePackageByIdQueryDto>
{
    public int Id { get; set; }
}
