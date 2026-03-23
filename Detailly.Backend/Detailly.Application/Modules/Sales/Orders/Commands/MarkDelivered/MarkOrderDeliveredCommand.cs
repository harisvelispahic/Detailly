namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkDelivered;

public sealed class MarkOrderDeliveredCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Note { get; set; }
}