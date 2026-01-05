namespace Detailly.Application.Abstractions.Payments;

public interface IStripeService
{
    Task<(string ProviderTransactionId, string ClientSecret)> CreatePaymentIntentAsync(
        decimal amount,
        int bookingId,
        CancellationToken ct);
}
