using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkShipped;

public sealed class MarkOrderShippedCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<MarkOrderShippedCommand>
{
    public async Task Handle(MarkOrderShippedCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isStaff = currentUser.IsAdmin || currentUser.IsManager || currentUser.IsEmployee;
        if (!isStaff)
            throw new DetaillyUnauthorizedException("Only staff can mark orders as shipped.");

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, ct);
        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        if (order.Status == OrderStatus.Cancelled)
            throw new DetaillyBusinessRuleException("order.cancelled", "Cancelled orders cannot be shipped.");

        if (order.Status != OrderStatus.Paid)
            throw new DetaillyBusinessRuleException("order.not_paid", "Only Paid orders can be marked as Shipped.");

        order.Status = OrderStatus.Shipped;

        var note = request.Note?.Trim();
        if (!string.IsNullOrWhiteSpace(note))
        {
            order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                ? $"[SHIPPED] {note}"
                : order.Notes + Environment.NewLine + $"[SHIPPED] {note}";
        }

        await context.SaveChangesAsync(ct);
    }
}