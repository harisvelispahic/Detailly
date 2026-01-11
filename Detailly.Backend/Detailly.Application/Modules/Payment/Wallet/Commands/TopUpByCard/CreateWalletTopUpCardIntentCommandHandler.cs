using Detailly.Application.Abstractions;
using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUpByCard;

public class CreateWalletTopUpCardIntentCommandHandler
    : IRequestHandler<CreateWalletTopUpCardIntentCommand, CreateWalletTopUpCardIntentResult>
{
    private readonly IAppDbContext _context;
    private readonly IStripeService _stripe;

    public CreateWalletTopUpCardIntentCommandHandler(IAppDbContext context, IStripeService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    public async Task<CreateWalletTopUpCardIntentResult> Handle(
        CreateWalletTopUpCardIntentCommand request,
        CancellationToken ct)
    {
        if (request.Amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        var wallet = await _context.Wallet
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId, ct)
            ?? throw new Exception("Wallet not found.");

        // Create Stripe PaymentIntent (reuse existing method but pass a dummy bookingId? NO)
        // -> We'll add a new overload in IStripeService (next step).
        var (providerTransactionId, clientSecret) =
            await _stripe.CreateWalletTopUpPaymentIntentAsync(
                request.Amount,
                wallet.Id,
                request.UserId,
                ct);

        var transaction = new PaymentTransactionEntity
        {
            Amount = request.Amount,
            TransactionType = TransactionType.Deposit,
            Status = PaymentTransactionStatus.Pending,
            TransactionDate = DateTime.UtcNow,

            Provider = "Stripe",
            ProviderTransactionId = providerTransactionId,
            Description = request.Description ?? "Wallet top-up (card)",

            WalletId = wallet.Id
        };

        _context.PaymentTransactions.Add(transaction);
        await _context.SaveChangesAsync(ct);

        return new CreateWalletTopUpCardIntentResult
        {
            ClientSecret = clientSecret
        };
    }
}
