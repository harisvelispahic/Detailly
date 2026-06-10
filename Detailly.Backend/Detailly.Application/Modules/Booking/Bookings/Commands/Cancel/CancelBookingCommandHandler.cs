using Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IMediator mediator,
    TimeProvider timeProvider)
    : IRequestHandler<CancelBookingCommand, Unit>
{
    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var customerId = authService.RequireUserId();

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.CustomerId != customerId)
            throw new DetaillyBusinessRuleException("BOOKING_FORBIDDEN", "You can only cancel your own booking.");

        // Terminal states (idempotent)
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed or BookingStatus.Expired)
            return Unit.Value;

        // Allowed: Draft, PendingPayment, Confirmed
        if (booking.Status is not (BookingStatus.Draft or BookingStatus.PendingPayment or BookingStatus.Confirmed))
            throw new DetaillyBusinessRuleException("BOOKING_NOT_CANCELLABLE", "Booking cannot be cancelled in its current state.");

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        if (booking.Status == BookingStatus.Confirmed)
        {
            var paidPayment = await context.PaymentTransactions
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.BookingId == booking.Id &&
                    x.TransactionType == TransactionType.Payment &&
                    x.Status == PaymentTransactionStatus.Paid)
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (paidPayment is null)
                throw new DetaillyBusinessRuleException("BOOKING_PAYMENT_MISSING",
                    "Cannot refund: booking has no paid payment transaction.");

            var refundPercent = GetRefundPercent(now, booking.StartUtc);
            var refundAmount = Math.Round(booking.TotalPrice * refundPercent, 2, MidpointRounding.AwayFromZero);

            if (refundAmount > 0)
            {
                if (paidPayment.WalletId is not null || string.Equals(paidPayment.Provider, "Wallet", StringComparison.OrdinalIgnoreCase))
                {
                    await mediator.Send(new RefundWalletPaymentCommand(paidPayment.Id, refundAmount), ct);
                }
                else if (string.Equals(paidPayment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase))
                {
                    await mediator.Send(new RefundStripePaymentCommand(paidPayment.Id, refundAmount), ct);
                }
                else
                {
                    throw new DetaillyBusinessRuleException("PAYMENT_PROVIDER_UNKNOWN",
                        "Unknown payment provider; cannot refund.");
                }
            }
        }

        booking.Status = BookingStatus.Cancelled;
        booking.ReservationExpiresAtUtc = null;
        booking.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Unit.Value;
    }

    private static decimal GetRefundPercent(DateTime nowUtc, DateTime startUtc)
    {
        var hours = (startUtc - nowUtc).TotalHours;

        if (hours >= 48) return 1.00m;
        if (hours >= 24) return 0.50m;
        if (hours > 0) return 0.25m;

        return 0.00m;
    }
}
