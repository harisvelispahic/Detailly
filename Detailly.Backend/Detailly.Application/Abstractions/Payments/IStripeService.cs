namespace Detailly.Application.Abstractions.Payments;

public interface IStripeService
{
    Task<(string ProviderTransactionId, string ClientSecret)> CreatePaymentIntentAsync(
        decimal amount,
        int bookingId,
        CancellationToken ct);

    Task<(string ProviderTransactionId, string ClientSecret)> CreateWalletTopUpPaymentIntentAsync(
        decimal amount,
        int walletId,
        int userId,
        CancellationToken ct);

}
