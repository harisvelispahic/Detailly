
using Detailly.Application.Abstractions.Payments;

namespace Detailly.Infrastructure.Payments.Stripe;


//public class FakeStripeService : IStripeService
public class FakeStripeService
{
    public Task<(string ProviderTransactionId, string ClientSecret)> CreatePaymentIntentAsync(
        decimal amount,
        int bookingId,
        CancellationToken ct)
    {
        var id = Guid.NewGuid().ToString();

        return Task.FromResult(
            (ProviderTransactionId: id,
             ClientSecret: $"fake_client_secret_{id}")
        );
    }
}
