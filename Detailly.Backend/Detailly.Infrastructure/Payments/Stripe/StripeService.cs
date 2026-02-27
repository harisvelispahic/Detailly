
using Detailly.Application.Abstractions.Payments;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Detailly.Infrastructure.Payments.Stripe;

public class StripeService : IStripeService
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;

    public StripeService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        _paymentIntentService = new PaymentIntentService();
        _refundService = new RefundService();
    }

    public async Task<(string ProviderTransactionId, string ClientSecret)>
        CreatePaymentIntentAsync(decimal amount, int bookingId, CancellationToken ct)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero), // cents
            Currency = "bam",
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() }
            }
        };

        var intent = await _paymentIntentService.CreateAsync(options, cancellationToken: ct);

        return (intent.Id, intent.ClientSecret);
    }

    public async Task<(string ProviderTransactionId, string ClientSecret)>
        CreateWalletTopUpPaymentIntentAsync(decimal amount, int walletId, int userId, CancellationToken ct)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero),
            Currency = "bam",
            Metadata = new Dictionary<string, string>
            {
                { "walletId", walletId.ToString() },
                { "userId", userId.ToString() },
                { "purpose", "wallet_topup" }
            }
        };

        var intent = await _paymentIntentService.CreateAsync(options, cancellationToken: ct);
        return (intent.Id, intent.ClientSecret);
    }

    // NEW: Refund Stripe PaymentIntent (partial or full)
    public async Task RefundPaymentIntentAsync(string providerTransactionId, decimal amount, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(providerTransactionId))
            throw new ArgumentException("ProviderTransactionId is required.", nameof(providerTransactionId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Refund amount must be greater than zero.");

        var options = new RefundCreateOptions
        {
            PaymentIntent = providerTransactionId,
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero) // cents
        };

        await _refundService.CreateAsync(options, cancellationToken: ct);
    }
}