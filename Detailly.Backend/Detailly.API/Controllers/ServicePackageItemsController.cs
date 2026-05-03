using Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicePackageItemsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListServicePackageItemsQueryDto>> List(
        [FromQuery] ListServicePackageItemsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
