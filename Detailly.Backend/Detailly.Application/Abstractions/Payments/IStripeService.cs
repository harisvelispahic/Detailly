namespace Detailly.Application.Abstractions.Payments;

public interface IStripeService
{
    Task<(string ProviderTransactionId, string ClientSecret)> CreateBookingPaymentIntentAsync(
        decimal amount,
        int bookingId,
        CancellationToken ct);

    Task<(string ProviderTransactionId, string ClientSecret)> CreateOrderPaymentIntentAsync(
    decimal amount,
    int orderId,
    CancellationToken ct);

    Task<(string ProviderTransactionId, string ClientSecret)> CreateWalletTopUpPaymentIntentAsync(
        decimal amount,
        int walletId,
        int userId,
        CancellationToken ct);

    Task<string> RefundPaymentIntentAsync(
        string paymentIntentId,
        decimal amount,
        CancellationToken ct);
}
