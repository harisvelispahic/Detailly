namespace Detailly.Application.Modules.Sales.Carts.Queries.GetMyCart;

public sealed class GetMyCartQueryDto
{
    public int CartId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsEmpty { get; set; }
    public string Status { get; set; } = default!;

    public List<GetMyCartItemQueryDto> Items { get; set; } = new();
    // returns the products that the user has saved for later, which may or may not be in the cart currently. This is useful for showing the user what they have saved even if they haven't added it to the cart yet, or if they removed it from the cart but still want to keep it for later.
    public List<GetMySavedProductInCartViewQueryDto> SavedProducts { get; set; } = new();
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

public sealed class GetMySavedProductInCartViewQueryDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
    public bool ProductIsEnabled { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime SavedAtUtc { get; set; }
}