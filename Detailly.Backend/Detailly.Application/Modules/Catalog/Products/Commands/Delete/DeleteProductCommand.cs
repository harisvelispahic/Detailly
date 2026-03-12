namespace Detailly.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
