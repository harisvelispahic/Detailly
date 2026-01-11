
using Detailly.Application.Abstractions;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using Detailly.Application.Abstractions.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;
public class CreateCardPaymentIntentCommandHandler
    : IRequestHandler<CreateCardPaymentIntentCommand, CreateCardPaymentIntentResult>
{
    private readonly IAppDbContext _context;
    private readonly IStripeService _stripe;

    public CreateCardPaymentIntentCommandHandler(
        IAppDbContext context,
        IStripeService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    public async Task<CreateCardPaymentIntentResult> Handle(
        CreateCardPaymentIntentCommand request,
        CancellationToken ct)
    {


        // TEMP DEBUG
        
        var bookingCount = await _context.Bookings.CountAsync(ct);
        Console.WriteLine("EF SEES BOOKINGS COUNT = " + bookingCount);

        Console.WriteLine($"Incoming BookingId = {request.BookingId}");




        // 1) Load booking
        var booking = await _context.Bookings
            .Include(b => b.PaymentTransaction)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, ct)
            ?? throw new Exception("Booking not found.");

        if (booking.ApplicationUserId != request.UserId)
            throw new Exception("Forbidden.");

        if (booking.Status != BookingStatus.PendingPayment)
            throw new Exception("Booking is not awaiting payment.");

        if (booking.PaymentTransaction is not null)
            throw new Exception("Payment already exists for this booking.");

        // 2) Call Stripe service
        var (providerTransactionId, clientSecret) =
            await _stripe.CreatePaymentIntentAsync(
                booking.TotalPrice,
                booking.Id,
                ct);

        // 3) Create PaymentTransaction as Pending
        var transaction = new PaymentTransactionEntity
        {
            Amount = booking.TotalPrice,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Pending,
            TransactionDate = DateTime.UtcNow,

            Provider = "Stripe",
            ProviderTransactionId = providerTransactionId,
            Description = "Card payment intent created",

            Booking = booking
        };

        _context.PaymentTransactions.Add(transaction);
        booking.PaymentTransaction = transaction;

        await _context.SaveChangesAsync(ct);

        // 4) Return client secret to frontend
        return new CreateCardPaymentIntentResult
        {
            ClientSecret = clientSecret
        };
    }
}