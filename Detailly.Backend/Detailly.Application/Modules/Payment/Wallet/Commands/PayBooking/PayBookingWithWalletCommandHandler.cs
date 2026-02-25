
using Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

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

        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        var booking = await _context.Bookings
            .Include(x => x.PaymentTransaction)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId && !x.IsDeleted, ct)
            ?? throw new Exception("Booking not found.");

        if (booking.CustomerId != request.UserId)
            throw new Exception("Forbidden.");

        // idempotent success
        if (booking.Status == BookingStatus.Confirmed)
        {
            await tx.CommitAsync(ct);
            return Unit.Value;
        }

        if (booking.Status != BookingStatus.PendingPayment)
            throw new Exception("Booking is not awaiting payment.");

        // HOLD EXPIRY CHECK
        if (booking.ReservationExpiresAtUtc is null || booking.ReservationExpiresAtUtc <= now)
            throw new Exception("Booking reservation expired.");

        if (booking.PaymentTransaction is not null)
            throw new Exception("Payment already exists for this booking.");

        var wallet = await _context.Wallet
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId && !x.IsDeleted, ct)
            ?? throw new Exception("Wallet not found.");

        if (wallet.Balance < booking.TotalPrice)
            throw new Exception("Insufficient wallet balance.");

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
        booking.PaymentTransaction = payment;

        await _context.SaveChangesAsync(ct);

        // Centralized booking confirmation logic (clears expiry, enforces rules)
        await _mediator.Send(new ConfirmBookingAfterPaymentCommand(payment.Id), ct);

        await tx.CommitAsync(ct);
        return Unit.Value;
    }
}