namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Status.Disable;

public sealed class DisableProductCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
