
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Delete;
public class DeleteOrderCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteOrderCommand, Unit>
{
    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken ct)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Cancelled)
            throw new InvalidOperationException("Only orders in Draft/Cancelled status can be deleted.");

        context.Orders.Remove(order);

        if (order.OrderItems is not null)
        {
            foreach (var item in order.OrderItems)
            {
                context.OrderItems.Remove(item);
            }
        }

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
