namespace Detailly.Application.Modules.Sales.Carts.Queries.GetMyCart;

public sealed class GetMyCartQueryDto
{
    public int CartId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsEmpty { get; set; }
    public string Status { get; set; } = default!;

    public List<GetMyCartItemQueryDto> Items { get; set; } = new();
}

public sealed class GetMyCartItemQueryDto
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }

    public bool ProductIsEnabled { get; set; }
    public int QuantityInStock { get; set; }
}