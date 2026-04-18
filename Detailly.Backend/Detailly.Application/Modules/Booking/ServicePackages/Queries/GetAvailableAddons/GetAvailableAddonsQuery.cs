namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQuery : BasePagedQuery<GetAvailableAddonsQueryDto>
{
    public required int ServicePackageId { get; set; }
}