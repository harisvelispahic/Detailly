
using Detailly.Application.Modules.Payment.Card.Commands.RefundCardPayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser, IMediator mediator)
    : IRequestHandler<CancelBookingCommand, Unit>
{
    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var booking = await context.Bookings
            .Include(b => b.PaymentTransaction)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.CustomerId != customerId)
            throw new DetaillyBusinessRuleException("BOOKING_FORBIDDEN", "You can only cancel your own booking.");

        // Terminal states (idempotent)
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed or BookingStatus.Expired)
        {
            await tx.CommitAsync(ct);
            return Unit.Value;
        }

        // Allowed: Draft, PendingPayment, Confirmed
        if (booking.Status is not (BookingStatus.Draft or BookingStatus.PendingPayment or BookingStatus.Confirmed))
            throw new DetaillyBusinessRuleException("BOOKING_NOT_CANCELLABLE", "Booking cannot be cancelled in its current state.");

        // Refund policy applies ONLY if already paid/confirmed
        if (booking.Status == BookingStatus.Confirmed)
        {
            var payment = booking.PaymentTransaction;

            if (payment is null || payment.Status != PaymentTransactionStatus.Paid)
                throw new DetaillyBusinessRuleException("BOOKING_PAYMENT_MISSING", "Cannot refund: booking has no paid payment transaction.");

            var refundPercent = GetRefundPercent(now, booking.StartUtc);
            var refundAmount = Math.Round(booking.TotalPrice * refundPercent, 2, MidpointRounding.AwayFromZero);

            if (refundAmount > 0)
            {
                if (payment.WalletId is not null || string.Equals(payment.Provider, "Wallet", StringComparison.OrdinalIgnoreCase))
                {
                    await mediator.Send(new RefundWalletPaymentCommand(payment.Id, refundAmount), ct);
                }
                else if (string.Equals(payment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase))
                {
                    await mediator.Send(new RefundCardPaymentCommand(payment.Id, refundAmount), ct);
                }
                else
                {
                    throw new DetaillyBusinessRuleException("PAYMENT_PROVIDER_UNKNOWN", "Unknown payment provider; cannot refund.");
                }
            }
        }

        booking.Status = BookingStatus.Cancelled;
        booking.ReservationExpiresAtUtc = null;
        booking.ModifiedAtUtc = now;

        // If you have a field for cancellation reason, store it (otherwise remove)
        // booking.CancellationReason = request.Reason?.Trim();

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