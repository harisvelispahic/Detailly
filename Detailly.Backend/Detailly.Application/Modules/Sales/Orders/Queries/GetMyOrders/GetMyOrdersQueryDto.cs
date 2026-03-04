namespace Detailly.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = default!;
}