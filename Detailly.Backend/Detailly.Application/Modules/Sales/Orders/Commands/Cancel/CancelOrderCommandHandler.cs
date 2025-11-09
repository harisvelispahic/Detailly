
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;
public class CancelOrderCommandHandler(IAppDbContext ctx) : IRequestHandler<CancelOrderCommand, Unit>
{
    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await ctx.Orders.FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new DetaillyNotFoundException($"Order {request.Id} not found.");

        if (order.Status is OrderStatus.Shipped or OrderStatus.Cancelled)
            throw new DetaillyConflictException("Cannot cancel a shipped or already canceled order.");

        order.Status = OrderStatus.Cancelled;
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
