
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Place;
public class PlaceOrderCommandHandler(IAppDbContext ctx) 
    : IRequestHandler<PlaceOrderCommand, Unit>
{
    public async Task<Unit> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var order = await ctx.Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new DetaillyNotFoundException($"Order {request.Id} not found.");
        
        if (order.Status != OrderStatus.Draft)
            throw new DetaillyConflictException("Only draft orders can be placed.");

        if (!order.OrderItems.Any())
            throw new DetaillyConflictException("Cannot place an order without order items.");

        order.Status = OrderStatus.Pending;
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}