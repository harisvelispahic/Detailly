//using Detailly.Application.Modules.Catalog.Products.Commands.Delete;
//using Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;
//using Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;
using Detailly.Application.Modules.Catalog.Products.Commands.Create;
//using Detailly.Application.Modules.Catalog.Products.Commands.Update;
using Detailly.Application.Modules.Catalog.Products.Queries.GetById;
using Detailly.Application.Modules.Catalog.Products.Queries.List;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateProduct(CreateProductCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    //[HttpPut("{id:int}")]
    //public async Task Update(int id, UpdateProductCommand command, CancellationToken ct)
    //{
    //    // ID from the route takes precedence
    //    command.Id = id;
    //    await sender.Send(command, ct);
    //    // no return -> 204 No Content
    //}

    //[HttpDelete("{id:int}")]
    //public async Task Delete(int id, CancellationToken ct)
    //{
    //    await sender.Send(new DeleteProductCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}

    [HttpGet("{id:int}")]
    public async Task<GetProductByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var product = await sender.Send(new GetProductByIdQuery { Id = id }, ct);
        return product; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<PageResult<ListProductsQueryDto>> List([FromQuery] ListProductsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    //[HttpPut("{id:int}/disable")]
    //public async Task Disable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new DisableProductCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}

    //[HttpPut("{id:int}/enable")]
    //public async Task Enable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new EnableProductCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}
}