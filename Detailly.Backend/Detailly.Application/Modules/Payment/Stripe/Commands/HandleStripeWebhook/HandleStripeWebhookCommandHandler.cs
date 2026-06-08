using Detailly.Application.Abstractions.Payments;
using Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;
using Detailly.Application.Modules.Sales.Orders.Commands.ConfirmAfterPayment;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using Microsoft.Extensions.Configuration;

namespace Detailly.Application.Modules.Payment.Card.Commands.HandleStripeWebhook;

public class HandleStripeWebhookCommandHandler
    : IRequestHandler<HandleStripeWebhookCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IStripeWebhookParser _parser;
    private readonly IMediator _mediator;

    public HandleStripeWebhookCommandHandler(
        IAppDbContext context,
        IConfiguration config,
        IStripeWebhookParser parser,
        IMediator mediator)
    {
        _context = context;
        _config = config;
        _parser = parser;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken ct)
    {
        var secret = _config["Stripe:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(secret))
            return Unit.Value;

        //
        // 0️⃣ Verify signature + parse webhook (EventUtility.ConstructEvent handles both)
        //
        var parsed = _parser.Parse(request.Payload, request.SignatureHeader ?? "", secret);

        if (parsed is null)
            return Unit.Value;

        var (eventId, eventType, providerTransactionId) = parsed.Value;

        //
        // 2️⃣ Idempotency
        //
        var alreadyProcessed = await _context.ProcessedWebhookEvents
            .AnyAsync(x => x.EventId == eventId, ct);

        if (alreadyProcessed)
            return Unit.Value;

        //
        // 3️⃣ Look up our payment
        //
        var payment = await _context.PaymentTransactions
            .Include(x => x.Booking)
            .Include(x => x.Wallet).ThenInclude(w => w!.ApplicationUser)
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.ProviderTransactionId == providerTransactionId && !x.IsDeleted, ct);

        if (payment is null)
        {
            // Still store processed event so Stripe doesn't keep retrying forever
            _context.ProcessedWebhookEvents.Add(new ProcessedWebhookEventEntity { EventId = eventId });
            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }

        //
        // 4️⃣ Apply transitions
        //
        if (eventType == "payment_intent.succeeded")
        {
            // prevent double-applying if already paid
            if (payment.Status != PaymentTransactionStatus.Paid)
            {
                payment.Status = PaymentTransactionStatus.Paid;

                // Booking payment: use centralized confirm logic
                // (checks hold expiry + clears ReservationExpiresAtUtc)
                if (payment.BookingId is not null)  // used to be payment.Booking is not null
                {
                    await _mediator.Send(new ConfirmBookingAfterPaymentCommand(payment.Id), ct);
                }

                if (payment.OrderId is not null)    // used to be payment.Order is not null
                {
                    await _mediator.Send(new ConfirmOrderAfterPaymentCommand(payment.Id), ct);
                }

                // Wallet top-up flow
                if (payment.Wallet is not null && payment.TransactionType == TransactionType.Deposit)
                {
                    var wallet = payment.Wallet;

                    var settings = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(ct);
                    var bonusPercent = (wallet.ApplicationUser.IsFleet
                        ? settings?.FleetWalletBonusPercent
                        : settings?.StandardWalletBonusPercent) ?? 0;

                    var bonus = (bonusPercent / 100m) * payment.Amount;

                    wallet.Balance += payment.Amount + bonus;
                    wallet.TotalDeposited += payment.Amount;

                    payment.Description = (payment.Description ?? "Wallet top-up") + $" (+{bonus:0.00} bonus)";
                }
            }
        }
        else if (eventType == "payment_intent.payment_failed")
        {
            if (payment.Status != PaymentTransactionStatus.Failed)
            {
                payment.Status = PaymentTransactionStatus.Failed;

                // IMPORTANT:
                // Do NOT cancel the booking here.
                // Keep it PendingPayment so the user can retry with another card until ReservationExpiresAtUtc.
            }
        }
        else
        {
            // Ignore other event types (still record idempotency below)
        }

        //
        // 5️⃣ Save processed event
        //
        _context.ProcessedWebhookEvents.Add(new ProcessedWebhookEventEntity
        {
            EventId = eventId
        });

        //
        // 6️⃣ Persist
        //
        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}