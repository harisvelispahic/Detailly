namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;

public sealed class CancelOrderCommand : IRequest
{
    public int Id { get; set; }
    public string? Reason { get; set; }
}