using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Detailly.Infrastructure.Background;

public sealed class BookingHoldExpiryCleanupService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingHoldExpiryCleanupService> _logger;
    private readonly TimeProvider _timeProvider;

    public BookingHoldExpiryCleanupService(IServiceScopeFactory scopeFactory, ILogger<BookingHoldExpiryCleanupService> logger, TimeProvider timeProvider)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Periodic loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(Interval, stoppingToken);
                await ExpireHoldsAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking hold expiry cleanup failed.");
            }
        }
    }

    private async Task ExpireHoldsAsync(CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var stripeService = scope.ServiceProvider.GetRequiredService<IStripeService>();

        // Find holds that are still PendingPayment but expired
        var expired = await db.Bookings
            .Include(b => b.PaymentTransactions)
            .Where(b =>
                !b.IsDeleted &&
                b.Status == BookingStatus.PendingPayment &&
                b.ReservationExpiresAtUtc != null &&
                b.ReservationExpiresAtUtc <= now)
            .ToListAsync(ct);

        if (expired.Count == 0)
            return;

        foreach (var b in expired)
        {
            b.Status = BookingStatus.Expired;
            b.ReservationExpiresAtUtc = null;
            b.ModifiedAtUtc = now;

            foreach (var tx in b.PaymentTransactions)
            {
                if (tx.Status == PaymentTransactionStatus.Pending)
                {
                    // Release the authorization hold on Stripe so the customer's card
                    // is freed immediately rather than waiting for the 7-day auto-expiry.
                    if (!string.IsNullOrWhiteSpace(tx.ProviderTransactionId))
                    {
                        try
                        {
                            await stripeService.CancelPaymentIntentAsync(tx.ProviderTransactionId, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Failed to cancel PaymentIntent {IntentId} for expired booking {BookingId}.",
                                tx.ProviderTransactionId, b.Id);
                        }
                    }

                    tx.Status = PaymentTransactionStatus.Unpaid;
                }
            }
        }

        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Expired {Count} booking hold(s).", expired.Count);
    }
}