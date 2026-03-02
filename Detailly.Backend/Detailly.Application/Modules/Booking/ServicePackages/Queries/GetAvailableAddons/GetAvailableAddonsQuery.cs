using MediatR;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQuery : IRequest<List<GetAvailableAddonsQueryDto>>
{
    public required int ServicePackageId { get; set; }
}