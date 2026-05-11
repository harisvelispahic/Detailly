using Detailly.Application.Abstractions;
using Detailly.Application.Common.Exceptions;
using Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using MediatR;

namespace Detailly.Tests.BookingCancellationTests.UnitTests;

public class BookingCancellationUnitTests
{
    // ---- Test doubles ----

    private sealed class FakeCurrentUser(int userId) : IAppCurrentUser
    {
        public int? ApplicationUserId => userId;
        public string? Email => null;
        public bool IsAuthenticated => true;
        public bool IsAdmin => false;
        public bool IsManager => false;
        public bool IsEmployee => false;
        public bool IsFleet => false;
    }

    private sealed class UnauthenticatedUser : IAppCurrentUser
    {
        public int? ApplicationUserId => null;
        public string? Email => null;
        public bool IsAuthenticated => false;
        public bool IsAdmin => false;
        public bool IsManager => false;
        public bool IsEmployee => false;
        public bool IsFleet => false;
    }

    private sealed class SpyMediator : IMediator
    {
        public List<object> SentRequests { get; } = [];

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
        {
            SentRequests.Add(request);
            return Task.FromResult(default(TResponse)!);
        }

        public Task Send<TRequest>(TRequest request, CancellationToken ct = default) where TRequest : IRequest
        {
            SentRequests.Add(request);
            return Task.CompletedTask;
        }

        public Task<object?> Send(object request, CancellationToken ct = default)
        {
            SentRequests.Add(request);
            return Task.FromResult<object?>(null);
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken ct = default)
            => throw new NotImplementedException();

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task Publish(object notification, CancellationToken ct = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken ct = default) where TNotification : INotification
            => Task.CompletedTask;
    }

    // ---- Helpers ----

    private static DatabaseContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var clock = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        return new DatabaseContext(options, clock);
    }

    private static async Task<BookingEntity> CreateBookingAsync(
        DatabaseContext context,
        BookingStatus status,
        int customerId,
        DateTime? startUtc = null)
    {
        var start = startUtc ?? DateTime.UtcNow.AddHours(72);
        var booking = new BookingEntity
        {
            CustomerId = customerId,
            ShopLocationId = 1,
            ServicePackageId = 1,
            TotalPrice = 100m,
            StartUtc = start,
            EndUtc = start.AddHours(2),
            RequiredEmployees = 1,
            RequiredBays = 1,
            Status = status,
            ServiceMode = ServiceMode.InShop,
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync(CancellationToken.None);
        return booking;
    }

    private static async Task<PaymentTransactionEntity> CreatePaidWalletPaymentAsync(
        DatabaseContext context,
        int bookingId,
        decimal amount)
    {
        var payment = new PaymentTransactionEntity
        {
            Amount = amount,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Paid,
            TransactionDate = DateTime.UtcNow.AddDays(-1),
            BookingId = bookingId,
            WalletId = 1,
            Provider = "Wallet",
            // Unique value avoids InMemory unique-index collision across tests that share a database name
            ProviderTransactionId = Guid.NewGuid().ToString(),
        };
        context.PaymentTransactions.Add(payment);
        await context.SaveChangesAsync(CancellationToken.None);
        return payment;
    }

    private static CancelBookingCommandHandler MakeHandler(
        DatabaseContext context,
        IAppCurrentUser user,
        SpyMediator mediator)
        => new(context, user, mediator);

    // =====================
    // Refund tier tests
    // =====================

    [Fact]
    public async Task Handle_ShouldRefund100Percent_WhenStartIsMoreThan48HoursAway()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(72));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(100m, refund.Amount); // 100% of 100 m
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public async Task Handle_ShouldRefund50Percent_WhenStartIsBetween24And48HoursAway()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(36));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(50m, refund.Amount); // 50% of 100 m
    }

    [Fact]
    public async Task Handle_ShouldRefund25Percent_WhenStartIsLessThan24HoursAway()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(12));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(25m, refund.Amount); // 25% of 100 m
    }

    [Fact]
    public async Task Handle_ShouldSkipRefund_WhenBookingAlreadyStarted()
    {
        using var context = GetInMemoryDbContext();
        // StartUtc is in the past → 0% refund → no mediator call
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(-1));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    // Boundary: just over 48 h falls into the ≥ 48 h tier → 100% of TotalPrice (100m)
    [Fact]
    public async Task Handle_ShouldRefund100Percent_WhenStartIsJustOver48Hours()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(48).AddMinutes(5));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(100m, refund.Amount); // 100% of booking.TotalPrice (100m)
    }

    // Boundary: just over 24 h falls into the 24–48 h tier → 50% of TotalPrice (100m)
    [Fact]
    public async Task Handle_ShouldRefund50Percent_WhenStartIsJustOver24Hours()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(24).AddMinutes(5));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(50m, refund.Amount); // 50% of booking.TotalPrice (100m)
    }

    // ========================
    // State machine tests
    // ========================

    [Fact]
    public async Task Handle_ShouldCancelDraftBooking_WithNoRefund()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Draft, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.Empty(spy.SentRequests);
    }

    [Fact]
    public async Task Handle_ShouldCancelPendingPaymentBooking_WithNoRefund()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.PendingPayment, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.Empty(spy.SentRequests);
    }

    [Fact]
    public async Task Handle_ShouldReturnSilently_WhenBookingIsAlreadyCancelled()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Cancelled, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Cancelled, booking.Status); // unchanged
    }

    [Fact]
    public async Task Handle_ShouldReturnSilently_WhenBookingIsCompleted()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Completed, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Completed, booking.Status); // unchanged
    }

    [Fact]
    public async Task Handle_ShouldReturnSilently_WhenBookingIsExpired()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Expired, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeCurrentUser(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Expired, booking.Status); // unchanged
    }

    // ========================
    // Ownership / auth tests
    // ========================

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserIsNotAuthenticated()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Draft, 1);

        var ex = await Assert.ThrowsAsync<DetaillyBusinessRuleException>(
            () => MakeHandler(context, new UnauthenticatedUser(), new SpyMediator())
                .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None));

        Assert.Equal("AUTH_REQUIRED", ex.Code);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBooking()
    {
        using var context = GetInMemoryDbContext();
        // Booking owned by user 1, attempting cancellation as user 2
        var booking = await CreateBookingAsync(context, BookingStatus.Draft, customerId: 1);

        var ex = await Assert.ThrowsAsync<DetaillyBusinessRuleException>(
            () => MakeHandler(context, new FakeCurrentUser(2), new SpyMediator())
                .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None));

        Assert.Equal("BOOKING_FORBIDDEN", ex.Code);
    }
}
