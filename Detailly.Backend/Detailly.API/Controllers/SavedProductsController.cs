using Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;
using Detailly.Application.Modules.Sales.SavedProducts.Commands.SaveProduct;
using Detailly.Application.Modules.Sales.SavedProducts.Queries.GetMySavedProducts;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = AuthPolicies.AnyClient)]
public sealed class SavedProductsController(ISender sender) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<List<GetMySavedProductsQueryDto>>> GetMy(CancellationToken ct)
        => Ok(await sender.Send(new GetMySavedProductsQuery(), ct));

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> Save(int productId, CancellationToken ct)
    {
        await sender.Send(new SaveProductCommand { ProductId = productId }, ct);
        return NoContent();
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId, CancellationToken ct)
    {
        await sender.Send(new RemoveSavedProductCommand { ProductId = productId }, ct);
        return NoContent();
    }
}