using Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;
using Detailly.Application.Modules.Sales.SavedProducts.Commands.SaveProduct;
using Detailly.Application.Modules.Sales.SavedProducts.Queries.ListMySavedProducts;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = AuthPolicies.Authenticated)]
public sealed class SavedProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Save([FromQuery] SaveProductCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return NoContent();
    }

    [HttpGet("my")]
    public async Task<PageResult<ListMySavedProductsQueryDto>> GetMy([FromQuery] ListMySavedProductsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromQuery] RemoveSavedProductCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return NoContent();
    }
}
