
namespace Detailly.Application.Modules.Sales.Orders.Queries.GetById;
public class GetOrderByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryDto>
{
    public async Task<GetOrderByIdQueryDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await context.Orders
            .Where(o => o.Id == request.Id)
            .Select(o => new GetOrderByIdQueryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Notes = o.Notes ?? "",
                Status = o.Status,
                ShipToAddress = new GetOrderByIdQueryDtoAddress
                {
                    Id = o.ShipToAddress.Id,
                    Street = o.ShipToAddress.Street,
                    City = o.ShipToAddress.City,
                    PostalCode = o.ShipToAddress.PostalCode,
                    Region = o.ShipToAddress.Region,
                    Country = o.ShipToAddress.Country
                },
                ApplicationUser = new GetOrderByIdQueryDtoApplicationUser
                {
                    Id = o.ApplicationUser.Id,
                    FirstName = o.ApplicationUser.FirstName,
                    LastName = o.ApplicationUser.LastName,
                    Username = o.ApplicationUser.Username,
                    Email = o.ApplicationUser.Email,
                    Phone = o.ApplicationUser.Phone ?? ""
                },
                OrderItems = o.OrderItems
                .Select(i => new GetOrderByIdQueryDtoOrderItem
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Currency = i.Currency,
                    Quantity = i.Quantity,
                    LineSubtotal = i.LineSubtotal,
                    DiscountPercentage = i.DiscountPercentage,
                    LineTotal = i.LineTotal,
                    Product = new GetOrderByIdQueryDtoProduct
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        ProductNumber = i.Product.ProductNumber,
                        Tags = i.Product.Tags ?? ""
                    }
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (order == null)
        {
            throw new DetaillyNotFoundException($"Order with Id {request.Id} not found.");
        }

        return order;
    }
}
