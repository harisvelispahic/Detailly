using Detailly.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;

public sealed class ConfirmBookingAfterPaymentCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider)
    : IRequestHandler<ConfirmBookingAfterPaymentCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmBookingAfterPaymentCommand request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var ownTx = context.Database.CurrentTransaction is null;
        IDbContextTransaction? tx = ownTx ? await context.Database.BeginTransactionAsync(ct) : null;
        await using (tx)
        {
            var payment = await context.PaymentTransactions
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct);

            if (payment is null)
                throw new DetaillyNotFoundException("Payment transaction not found.");

            if (payment.Status != PaymentTransactionStatus.Paid)
                throw new DetaillyBusinessRuleException("PAYMENT_NOT_PAID", "Payment is not marked as paid.");

            if (payment.Booking is null)
                return Unit.Value;

            var booking = payment.Booking;

            if (booking.Status == BookingStatus.Confirmed)
                return Unit.Value;

            if (booking.Status != BookingStatus.PendingPayment)
                throw new DetaillyBusinessRuleException("BOOKING_NOT_CONFIRMABLE", "Booking is not pending payment.");

            if (booking.ReservationExpiresAtUtc is null || booking.ReservationExpiresAtUtc <= now)
                throw new DetaillyBusinessRuleException("BOOKING_EXPIRED", "Booking reservation expired.");

            booking.Status = BookingStatus.Confirmed;
            booking.ReservationExpiresAtUtc = null;
            booking.ModifiedAtUtc = now;

            await context.SaveChangesAsync(ct);
            if (ownTx) await tx!.CommitAsync(ct);

            return Unit.Value;
        }
    }
}
