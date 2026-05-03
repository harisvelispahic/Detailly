namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;

public class ListServicePackageItemsQuery : BasePagedQuery<ListServicePackageItemsQueryDto>
{
    public string? Search { get; init; }
}
