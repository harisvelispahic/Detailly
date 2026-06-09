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
        CreateBookingPaymentIntentAsync(decimal amount, int bookingId, CancellationToken ct)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero), // cents
            Currency = "bam",
            CaptureMethod = "manual",
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() }
            }
        };

        var intent = await _paymentIntentService.CreateAsync(options, cancellationToken: ct);

        return (intent.Id, intent.ClientSecret);
    }

    public async Task<(string ProviderTransactionId, string ClientSecret)>
        CreateOrderPaymentIntentAsync(decimal amount, int orderId, CancellationToken ct)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero),
            Currency = "bam",
            Metadata = new Dictionary<string, string>
        {
            { "orderId", orderId.ToString() }
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

    public async Task CapturePaymentIntentAsync(string paymentIntentId, CancellationToken ct)
    {
        await _paymentIntentService.CaptureAsync(paymentIntentId, cancellationToken: ct);
    }

    public async Task CancelPaymentIntentAsync(string paymentIntentId, CancellationToken ct)
    {
        await _paymentIntentService.CancelAsync(paymentIntentId, cancellationToken: ct);
    }

    public async Task<string> RefundPaymentIntentAsync(string paymentIntentId, decimal amount, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
            throw new ArgumentException("ProviderTransactionId is required.", nameof(paymentIntentId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Refund amount must be greater than zero.");

        var refundService = new RefundService();
        var refund = await refundService.CreateAsync(new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
            Amount = (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero) // cents
        }, cancellationToken: ct);

        return refund.Id; // re_...
    }
}