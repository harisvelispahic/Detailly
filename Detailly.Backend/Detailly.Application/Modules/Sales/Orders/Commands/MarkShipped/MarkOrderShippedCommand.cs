namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkShipped;

public sealed class MarkOrderShippedCommand : IRequest
{
    public int Id { get; set; }
    public string? Note { get; set; }
}