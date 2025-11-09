
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Queries.List;
public class ListOrdersQueryDto
{
    public required int Id { get; init; }
    public required string OrderNumber { get; init; }
    public required DateTime OrderDate { get; init; }
    public required decimal TotalAmount { get; init; }
    public required string Notes { get; init; }
    public required OrderStatus Status { get; init; }

    public required ListOrdersQueryDtoAddress ShipToAddress { get; init; }
    public required ListOrdersQueryDtoApplicationUser ApplicationUser { get; init; }
    public required List<ListOrdersQueryDtoOrderItem> OrderItems { get; init; }
}

public class ListOrdersQueryDtoAddress
{
    public required int Id { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Region { get; init; }
    public required string Country { get; init; }
}

public class ListOrdersQueryDtoApplicationUser
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }

}

public class ListOrdersQueryDtoOrderItem
{
    public required int Id { get; init; }
    public required decimal UnitPrice { get; init; }
    public required CurrencyName Currency { get; init; }
    public required int Quantity { get; init; }
    public required decimal LineSubtotal { get; init; }
    public required decimal DiscountPercentage { get; init; }
    public required decimal LineTotal { get; init; }

    public required ListOrdersQueryDtoProduct Product { get; init; }
}

public class ListOrdersQueryDtoProduct
{
    public required int Id { get; init; }
    public required string ProductNumber { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Tags { get; set; }
}
