
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

    public BookingHoldExpiryCleanupService(IServiceScopeFactory scopeFactory, ILogger<BookingHoldExpiryCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
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
        var now = DateTime.UtcNow;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        // Find holds that are still PendingPayment but expired
        var expired = await db.Bookings
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
            b.ReservationExpiresAtUtc = null; // clean up hold field
            b.ModifiedAtUtc = now;
        }

        await db.SaveChangesAsync(ct);

        _logger.LogInformation("Expired {Count} booking hold(s).", expired.Count);
    }
}