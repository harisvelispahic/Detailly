using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Detailly.Application.Modules.Payment.Card.Commands.HandleStripeWebhook;

public class HandleStripeWebhookCommandHandler
    : IRequestHandler<HandleStripeWebhookCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly IWebhookVerifier _verifier;
    private readonly IConfiguration _config;
    private readonly IStripeWebhookParser _parser;

    public HandleStripeWebhookCommandHandler(
        IAppDbContext context,
        IWebhookVerifier verifier,
        IConfiguration config,
        IStripeWebhookParser parser)
    {
        _context = context;
        _verifier = verifier;
        _config = config;
        _parser = parser;
    }

    public async Task<Unit> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken ct)
    {
        //
        // 0️⃣ Verify webhook signature
        //
        var secret = _config["Stripe:WebhookSecret"];

        Console.WriteLine("WEBHOOK RECEIVED");
        Console.WriteLine("SIGNATURE: " + request.SignatureHeader);
        Console.WriteLine("SECRET: " + secret);


        if (!_verifier.Verify(request.Payload, request.SignatureHeader ?? "", secret!))
            return Unit.Value;

        Console.WriteLine("WEBHOOK: signature OK");

        //
        // 1️⃣ Parse webhook (abstracted — no Stripe SDK in Application)
        //
        var parsed = _parser.Parse(request.Payload);

        if (parsed is null)
            return Unit.Value;


        var (eventId, eventType, providerTransactionId) = parsed.Value;

        Console.WriteLine($"WEBHOOK: parsed {eventType} / {providerTransactionId}");


        //
        // 2️⃣ Idempotency
        //
        var alreadyProcessed = await _context.ProcessedWebhookEvents
            .AnyAsync(x => x.EventId == eventId, ct);

        if (alreadyProcessed)
            return Unit.Value;

        Console.WriteLine($"WEBHOOK: idempotency check — new event {eventId}");

        //
        // 3️⃣ Look up our payment
        //
        var payment = await _context.PaymentTransactions
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(
                x => x.ProviderTransactionId == providerTransactionId,
                ct);

        if (payment is null)
            return Unit.Value;

        Console.WriteLine($"WEBHOOK: found payment {payment.Id}");

        //
        // 4️⃣ Apply transitions
        //
        if (eventType == "payment_intent.succeeded")
        {
            payment.Status = PaymentTransactionStatus.Paid;

            if (payment.Booking is not null)
                payment.Booking.Status = BookingStatus.Confirmed;
        }
        else if (eventType == "payment_intent.payment_failed")
        {
            payment.Status = PaymentTransactionStatus.Failed;

            if (payment.Booking is not null)
                payment.Booking.Status = BookingStatus.Cancelled;
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
