
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Ship;
public class ShipOrderCommandHandler(IAppDbContext ctx) : IRequestHandler<ShipOrderCommand, Unit>
{
    public async Task<Unit> Handle(ShipOrderCommand request, CancellationToken ct)
    {
        var order = await ctx.Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new DetaillyNotFoundException($"Order {request.Id} not found.");

        if (order.Status != OrderStatus.Paid)
            throw new DetaillyConflictException("Only paid orders can be shipped.");

        order.Status = OrderStatus.Shipped;
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
