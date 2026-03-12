namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
