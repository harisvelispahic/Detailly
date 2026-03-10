using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;

public class PayBookingWithWalletCommandHandler
    : IRequestHandler<PayBookingWithWalletCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly IMediator _mediator;

    public PayBookingWithWalletCommandHandler(IAppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(PayBookingWithWalletCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // ✅ No explicit tx here (ConfirmAfterPayment may start its own)

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(x => x.Id == request.BookingId && !x.IsDeleted, ct)
            ?? throw new DetaillyNotFoundException("Booking not found.");

        if (booking.CustomerId != request.UserId)
            throw new DetaillyForbiddenException("Forbidden.");

        // idempotent success
        if (booking.Status == BookingStatus.Confirmed)
            return Unit.Value;

        if (booking.Status != BookingStatus.PendingPayment)
            throw new DetaillyBusinessRuleException("BOOKING_NOT_AWAITING_PAYMENT", "Booking is not awaiting payment.");

        if (booking.ReservationExpiresAtUtc is null || booking.ReservationExpiresAtUtc <= now)
            throw new DetaillyBusinessRuleException("BOOKING_RESERVATION_EXPIRED", "Booking reservation expired.");

        // ✅ Check existing latest PAYMENT attempt
        var latestAttempt = await _context.PaymentTransactions
            .Where(x =>
                !x.IsDeleted &&
                x.BookingId == booking.Id &&
                x.TransactionType == TransactionType.Payment)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (latestAttempt is not null)
        {
            if (latestAttempt.Status == PaymentTransactionStatus.Paid)
                return Unit.Value; // already paid, treat as idempotent

            if (latestAttempt.Status == PaymentTransactionStatus.Pending)
                throw new DetaillyBusinessRuleException("PAYMENT_IN_PROGRESS", "Payment is already in progress.");
            // Failed/Unpaid -> allow wallet payment
        }

        var wallet = await _context.Wallet
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId && !x.IsDeleted, ct)
            ?? throw new DetaillyNotFoundException("Wallet not found.");

        if (wallet.Balance < booking.TotalPrice)
            throw new DetaillyBusinessRuleException("WALLET_INSUFFICIENT_FUNDS", "Insufficient wallet balance.");

        wallet.Balance -= booking.TotalPrice;

        var payment = new PaymentTransactionEntity
        {
            Amount = booking.TotalPrice,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Paid,
            TransactionDate = now,

            Provider = "Wallet",
            Description = "Booking paid with wallet",

            WalletId = wallet.Id,
            BookingId = booking.Id
        };

        _context.PaymentTransactions.Add(payment);
        await _context.SaveChangesAsync(ct);

        // Centralized confirmation (clears expiry, enforces hold rules, idempotent)
        await _mediator.Send(new ConfirmBookingAfterPaymentCommand(payment.Id), ct);

        return Unit.Value;
    }
}