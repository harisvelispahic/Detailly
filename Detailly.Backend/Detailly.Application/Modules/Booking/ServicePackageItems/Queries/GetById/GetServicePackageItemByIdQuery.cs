namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.GetById;

public class GetServicePackageItemByIdQuery : IRequest<GetServicePackageItemByIdQueryDto>
{
    public required int Id { get; init; }
}
