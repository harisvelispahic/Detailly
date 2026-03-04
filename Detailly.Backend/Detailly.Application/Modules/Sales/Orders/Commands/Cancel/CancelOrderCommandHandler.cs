using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;

public sealed class CancelOrderCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager || appCurrentUser.IsEmployee;

        if (!isStaff && order.ApplicationUserId != userId)
            throw new DetaillyUnauthorizedException("You do not have access to this order.");

        // Business rule: only Pending can be cancelled by user
        if (order.Status != OrderStatus.PendingPayment)
            throw new DetaillyBusinessRuleException(
                "order.cannot_cancel",
                "Only orders in Pending status can be cancelled.");

        order.Status = OrderStatus.Cancelled;

        // Optional: append reason into notes (simple audit)
        var reason = request.Reason?.Trim();
        if (!string.IsNullOrWhiteSpace(reason))
        {
            var prefix = "[CANCELLED] ";
            order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                ? prefix + reason
                : order.Notes + Environment.NewLine + prefix + reason;
        }

        await context.SaveChangesAsync(ct);
    }
}