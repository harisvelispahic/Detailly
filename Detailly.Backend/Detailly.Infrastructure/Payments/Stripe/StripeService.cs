using Detailly.Application.Abstractions.Payments;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Detailly.Infrastructure.Payments.Stripe;

public class StripeService : IStripeService
{
    private readonly PaymentIntentService _paymentIntentService;

    public StripeService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        _paymentIntentService = new PaymentIntentService();
    }

    public async Task<(string ProviderTransactionId, string ClientSecret)>
        CreatePaymentIntentAsync(decimal amount, int bookingId, CancellationToken ct)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // cents
            Currency = "bam",              // or "usd" etc.
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() }
            }
        };

        var intent = await _paymentIntentService.CreateAsync(options, cancellationToken: ct);

        return (intent.Id, intent.ClientSecret);
    }
}
