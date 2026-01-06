using Detailly.Application.Common;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.List;

public class ListServicePackagesQuery
    : BasePagedQuery<ListServicePackagesQueryDto>
{
    public string? Search { get; init; }
}
