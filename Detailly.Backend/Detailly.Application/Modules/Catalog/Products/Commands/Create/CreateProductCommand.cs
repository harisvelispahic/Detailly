
namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommand : IRequest<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int CategoryId { get; set; }

}