namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.SaveProduct;

public sealed class SaveProductCommand : IRequest
{
    public int ProductId { get; set; }
}