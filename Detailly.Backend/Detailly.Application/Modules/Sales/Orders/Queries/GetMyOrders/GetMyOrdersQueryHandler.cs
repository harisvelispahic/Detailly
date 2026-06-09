namespace Detailly.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<GetMyOrdersQuery, PageResult<GetMyOrdersQueryDto>>
{
    public async Task<PageResult<GetMyOrdersQueryDto>> Handle(GetMyOrdersQuery request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var q = context.Orders
            .AsNoTracking()
            .Where(o => o.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(o => o.OrderNumber.Contains(s));
        }

        if (request.Status is not null)
            q = q.Where(o => o.Status == request.Status);

        var projected = q
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new GetMyOrdersQueryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString()
            });

        return await PageResult<GetMyOrdersQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}