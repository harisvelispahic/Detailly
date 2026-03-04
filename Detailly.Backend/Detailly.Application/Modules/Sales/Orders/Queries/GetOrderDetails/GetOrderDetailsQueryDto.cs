namespace Detailly.Application.Modules.Sales.Orders.Queries.GetOrderDetails;

public sealed class GetOrderDetailsQueryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = default!;
    public string? Notes { get; set; }

    public int ShipToAddressId { get; set; }

    public List<GetOrderDetailsItemQueryDto> Items { get; set; } = new();
}

public sealed class GetOrderDetailsItemQueryDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }

    public string ProductName { get; set; } = default!;

    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = default!;
    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }
}