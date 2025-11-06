
namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
