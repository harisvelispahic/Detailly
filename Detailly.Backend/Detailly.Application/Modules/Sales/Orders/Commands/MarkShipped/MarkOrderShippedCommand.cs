namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkShipped;

public sealed class MarkOrderShippedCommand : IRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Note { get; set; }
}