namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
