namespace Detailly.Application.Modules.Sales.SavedProducts.Commands.RemoveSavedProduct;

public sealed class RemoveSavedProductCommand : IRequest
{
    public int ProductId { get; set; }
}