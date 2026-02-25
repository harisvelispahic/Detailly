
using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;

public class CreateCardPaymentIntentCommandHandler
    : IRequestHandler<CreateCardPaymentIntentCommand, CreateCardPaymentIntentResult>
{
    private readonly IAppDbContext _context;
    private readonly IStripeService _stripe;

    public CreateCardPaymentIntentCommandHandler(IAppDbContext context, IStripeService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    public async Task<CreateCardPaymentIntentResult> Handle(
        CreateCardPaymentIntentCommand request,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // 1) Load booking
        var booking = await _context.Bookings
            .Include(b => b.PaymentTransaction)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct)
            ?? throw new Exception("Booking not found.");

        // 2) Ownership
        if (booking.CustomerId != request.UserId)
            throw new Exception("Forbidden.");

        // 3) Booking status
        if (booking.Status != BookingStatus.PendingPayment)
            throw new Exception("Booking is not awaiting payment.");

        // 4) HOLD EXPIRY CHECK
        if (booking.ReservationExpiresAtUtc is null || booking.ReservationExpiresAtUtc <= now)
            throw new Exception("Booking reservation expired.");

        // 5) Payment idempotency / retry rules
        if (booking.PaymentTransaction is not null)
        {
            var existing = booking.PaymentTransaction;

            if (existing.Status == PaymentTransactionStatus.Paid)
                throw new Exception("Booking is already paid.");

            if (existing.Status == PaymentTransactionStatus.Pending)
                throw new Exception("Payment is already in progress.");

            // Failed/Unpaid -> allow retry by creating a new payment intent
            if (existing.Status is PaymentTransactionStatus.Failed or PaymentTransactionStatus.Unpaid)
            {
                // Break the link so we can attach a new transaction to the booking.
                // Keep the old transaction row for audit/history.
                booking.PaymentTransaction = null;
            }
            else
            {
                // Future-proof: if a new status is added and not handled above
                throw new Exception("Booking cannot start a new payment at this time.");
            }
        }

        // 6) Call Stripe service to create a new PaymentIntent
        var (providerTransactionId, clientSecret) =
            await _stripe.CreatePaymentIntentAsync(
                booking.TotalPrice,
                booking.Id,
                ct);

        // 7) Create PaymentTransaction as Pending
        var transaction = new PaymentTransactionEntity
        {
            Amount = booking.TotalPrice,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Pending,
            TransactionDate = now,

            Provider = "Stripe",
            ProviderTransactionId = providerTransactionId,
            Description = "Card payment intent created",

            BookingId = booking.Id
        };

        _context.PaymentTransactions.Add(transaction);

        // attach to booking
        booking.PaymentTransaction = transaction;

        await _context.SaveChangesAsync(ct);

        // 8) Return client secret to frontend
        return new CreateCardPaymentIntentResult
        {
            ClientSecret = clientSecret
        };
    }
}