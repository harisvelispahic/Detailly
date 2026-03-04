namespace Detailly.Application.Modules.Sales.Orders.Queries.GetOrderDetails;

public sealed class GetOrderDetailsQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetOrderDetailsQuery, GetOrderDetailsQueryDto>
{
    public async Task<GetOrderDetailsQueryDto> Handle(GetOrderDetailsQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        // Ownership rule: client can only see own orders; staff can see all
        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager || appCurrentUser.IsEmployee;

        if (!isStaff && order.ApplicationUserId != userId)
            throw new DetaillyUnauthorizedException("You do not have access to this order.");

        return new GetOrderDetailsQueryDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            Notes = order.Notes,
            ShipToAddressId = order.ShipToAddressId,
            Items = order.OrderItems.Select(oi => new GetOrderDetailsItemQueryDto
            {
                OrderItemId = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                UnitPrice = oi.UnitPrice,
                Currency = oi.Currency.ToString(),
                Quantity = oi.Quantity,
                LineTotal = oi.LineTotal
            }).ToList()
        };
    }
}