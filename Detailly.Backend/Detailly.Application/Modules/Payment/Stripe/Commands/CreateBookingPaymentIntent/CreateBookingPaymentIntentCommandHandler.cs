using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateBookingPaymentIntent;

public class CreateBookingPaymentIntentCommandHandler
    : IRequestHandler<CreateBookingPaymentIntentCommand, CreateBookingPaymentIntentResult>
{
    // If a PaymentIntent has been Pending for longer than this, allow replacing it with a new one.
    private static readonly TimeSpan PendingReplaceAfter = TimeSpan.FromMinutes(2);

    private readonly IAppDbContext _context;
    private readonly IStripeService _stripe;

    public CreateBookingPaymentIntentCommandHandler(IAppDbContext context, IStripeService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    public async Task<CreateBookingPaymentIntentResult> Handle(CreateBookingPaymentIntentCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct)
            ?? throw new Exception("Booking not found.");

        if (booking.CustomerId != request.UserId)
            throw new Exception("Forbidden.");

        if (booking.Status != BookingStatus.PendingPayment)
            throw new Exception("Booking is not awaiting payment.");

        if (booking.ReservationExpiresAtUtc is null || booking.ReservationExpiresAtUtc <= now)
            throw new Exception("Booking reservation expired.");

        // ✅ Latest PAYMENT attempt for this booking (ignore refunds)
        var existing = await _context.PaymentTransactions
            .Where(x =>
                !x.IsDeleted &&
                x.BookingId == booking.Id &&
                x.TransactionType == TransactionType.Payment)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
        {
            if (existing.Status == PaymentTransactionStatus.Paid)
                throw new Exception("Booking is already paid.");

            if (existing.Status == PaymentTransactionStatus.Pending)
            {
                var age = now - existing.TransactionDate;

                if (age < PendingReplaceAfter)
                    throw new Exception("Payment is already in progress.");

                // stale pending -> mark failed and allow new intent
                existing.Status = PaymentTransactionStatus.Failed;
                existing.Description = (existing.Description ?? "Card payment")
                                       + " (auto-failed due to stale pending intent)";
                // no detaching needed; history is kept via BookingId
            }

            // Failed/Unpaid -> allow new intent
            if (existing.Status is not (PaymentTransactionStatus.Failed or PaymentTransactionStatus.Unpaid or PaymentTransactionStatus.Pending))
                throw new Exception("Booking cannot start a new payment at this time.");
        }

        var (providerTransactionId, clientSecret) =
            await _stripe.CreateBookingPaymentIntentAsync(booking.TotalPrice, booking.Id, ct);

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
        await _context.SaveChangesAsync(ct);

        return new CreateBookingPaymentIntentResult
        {
            ClientSecret = clientSecret
        };
    }
}