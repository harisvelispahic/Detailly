using Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleCategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<IList<ListVehicleCategoriesQueryDto>> List(CancellationToken ct)
    {
        return await sender.Send(new ListVehicleCategoriesQuery(), ct);
    }
}
