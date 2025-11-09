
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Pay;
public class PayOrderCommandHandler(IAppDbContext ctx) : IRequestHandler<PayOrderCommand, Unit>
{
    public async Task<Unit> Handle(PayOrderCommand request, CancellationToken ct)
    {
        var order = await ctx.Orders
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new DetaillyNotFoundException($"Order {request.Id} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new DetaillyConflictException("Only pending orders can be paid.");

        // Optionally store payment info in another entity
        order.Status = OrderStatus.Paid;
        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}