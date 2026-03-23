namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;

public sealed class CancelOrderCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Reason { get; set; }
}