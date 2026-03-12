namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
