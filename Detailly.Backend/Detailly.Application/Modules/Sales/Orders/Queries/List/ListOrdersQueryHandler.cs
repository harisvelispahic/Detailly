
namespace Detailly.Application.Modules.Sales.Orders.Queries.List;
public class ListOrdersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
{
    public async Task<PageResult<ListOrdersQueryDto>> Handle(
        ListOrdersQuery request, CancellationToken ct)
    {
        var q = ctx.Orders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => x.OrderNumber.Contains(request.Search));
        }

        var projectedQuery = q.OrderByDescending(x => x.OrderDate)
            .Select(o => new ListOrdersQueryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Notes = o.Notes ?? "",
                Status = o.Status,
                ShipToAddress = new ListOrdersQueryDtoAddress
                {
                    Id = o.ShipToAddress.Id,
                    Street = o.ShipToAddress.Street,
                    City = o.ShipToAddress.City,
                    PostalCode = o.ShipToAddress.PostalCode,
                    Region = o.ShipToAddress.Region,
                    Country = o.ShipToAddress.Country
                },
                ApplicationUser = new ListOrdersQueryDtoApplicationUser
                {
                    Id = o.ApplicationUser.Id,
                    FirstName = o.ApplicationUser.FirstName,
                    LastName = o.ApplicationUser.LastName,
                    Username = o.ApplicationUser.Username,
                    Email = o.ApplicationUser.Email,
                    Phone = o.ApplicationUser.Phone ?? ""
                },
                OrderItems = o.OrderItems
                .Select(i => new ListOrdersQueryDtoOrderItem
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Currency = i.Currency,
                    Quantity = i.Quantity,
                    LineSubtotal = i.LineSubtotal,
                    DiscountPercentage = i.DiscountPercentage,
                    LineTotal = i.LineTotal,
                    Product = new ListOrdersQueryDtoProduct
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        ProductNumber = i.Product.ProductNumber,
                        Tags = i.Product.Tags ?? ""
                    }
                }).ToList()
            });

        return await PageResult<ListOrdersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }

}

