namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Patch;

public sealed class PatchProductCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}