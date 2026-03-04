using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkDelivered;

public sealed class MarkOrderDeliveredCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<MarkOrderDeliveredCommand>
{
    public async Task Handle(MarkOrderDeliveredCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isStaff = currentUser.IsAdmin || currentUser.IsManager || currentUser.IsEmployee;
        if (!isStaff)
            throw new DetaillyUnauthorizedException("Only staff can mark orders as delivered.");

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, ct);
        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        if (order.Status == OrderStatus.Cancelled)
            throw new DetaillyBusinessRuleException("order.cancelled", "Cancelled orders cannot be delivered.");

        if (order.Status != OrderStatus.Shipped)
            throw new DetaillyBusinessRuleException("order.not_shipped", "Only Shipped orders can be marked as Delivered.");

        order.Status = OrderStatus.Delivered;

        var note = request.Note?.Trim();
        if (!string.IsNullOrWhiteSpace(note))
        {
            order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                ? $"[DELIVERED] {note}"
                : order.Notes + Environment.NewLine + $"[DELIVERED] {note}";
        }

        await context.SaveChangesAsync(ct);
    }
}