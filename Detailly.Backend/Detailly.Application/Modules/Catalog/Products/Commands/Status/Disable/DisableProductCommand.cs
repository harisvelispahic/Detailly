
namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
