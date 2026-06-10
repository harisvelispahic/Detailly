using Detailly.Application.Abstractions;
using Detailly.Application.Common.Exceptions;
using Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using MediatR;
using Microsoft.Data.Sqlite;

namespace Detailly.Tests.BookingCancellationTests.UnitTests;

public class BookingCancellationUnitTests
{
    // ---- Test doubles ----

    private sealed class FakeAuthorizationService(int? userId) : IAppAuthorizationService
    {
        public int RequireUserId() =>
            userId ?? throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Not authenticated.");

        public void EnsureAuthenticated()
        {
            if (userId is null) throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Not authenticated.");
        }

        public void EnsureOwnerOrStaff(int resourceOwnerId, string resourceName = "resource") => throw new NotImplementedException();
        public void EnsureOwnerOrAdmin(int resourceOwnerId, string resourceName = "resource") => throw new NotImplementedException();
        public void EnsureAdminOrManager() => throw new NotImplementedException();
        public void EnsureAnyStaff() => throw new NotImplementedException();
        public void EnsureOwnerOrAnyStaff(int resourceOwnerId, string resourceName = "resource") => throw new NotImplementedException();
        public void EnsureEmployee() => throw new NotImplementedException();
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

    // ---- SQLite in-memory context (supports transactions) ----

    private sealed class TestDbContext : DatabaseContext
    {
        private readonly SqliteConnection _connection;

        public TestDbContext() : this(OpenConnection()) { }

        private TestDbContext(SqliteConnection connection)
            : base(
                new DbContextOptionsBuilder<DatabaseContext>()
                    .UseSqlite(connection)
                    .Options,
                new Microsoft.Extensions.Time.Testing.FakeTimeProvider())
        {
            _connection = connection;
            Database.EnsureCreated();
        }

        private static SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            return conn;
        }

        public override void Dispose()
        {
            base.Dispose();
            _connection.Dispose();
        }
    }

    // ---- Helpers ----

    private static TestDbContext GetInMemoryDbContext() => new();

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
            ProviderTransactionId = Guid.NewGuid().ToString(),
        };
        context.PaymentTransactions.Add(payment);
        await context.SaveChangesAsync(CancellationToken.None);
        return payment;
    }

    private static CancelBookingCommandHandler MakeHandler(
        DatabaseContext context,
        IAppAuthorizationService authService,
        SpyMediator mediator)
        => new(context, authService, mediator);

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
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(100m, refund.Amount);
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public async Task Handle_ShouldRefund50Percent_WhenStartIsBetween24And48HoursAway()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(36));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(50m, refund.Amount);
    }

    [Fact]
    public async Task Handle_ShouldRefund25Percent_WhenStartIsLessThan24HoursAway()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(12));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(25m, refund.Amount);
    }

    [Fact]
    public async Task Handle_ShouldSkipRefund_WhenBookingAlreadyStarted()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(-1));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public async Task Handle_ShouldRefund100Percent_WhenStartIsJustOver48Hours()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(48).AddMinutes(5));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(100m, refund.Amount);
    }

    [Fact]
    public async Task Handle_ShouldRefund50Percent_WhenStartIsJustOver24Hours()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Confirmed, 1, DateTime.UtcNow.AddHours(24).AddMinutes(5));
        await CreatePaidWalletPaymentAsync(context, booking.Id, 100m);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        var refund = spy.SentRequests.OfType<RefundWalletPaymentCommand>().Single();
        Assert.Equal(50m, refund.Amount);
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
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
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
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
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
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnSilently_WhenBookingIsCompleted()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Completed, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Completed, booking.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnSilently_WhenBookingIsExpired()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Expired, 1);

        var spy = new SpyMediator();
        await MakeHandler(context, new FakeAuthorizationService(1), spy)
            .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None);

        Assert.Empty(spy.SentRequests);
        Assert.Equal(BookingStatus.Expired, booking.Status);
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
            () => MakeHandler(context, new FakeAuthorizationService(null), new SpyMediator())
                .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None));

        Assert.Equal("AUTH_REQUIRED", ex.Code);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserDoesNotOwnBooking()
    {
        using var context = GetInMemoryDbContext();
        var booking = await CreateBookingAsync(context, BookingStatus.Draft, customerId: 1);

        var ex = await Assert.ThrowsAsync<DetaillyBusinessRuleException>(
            () => MakeHandler(context, new FakeAuthorizationService(2), new SpyMediator())
                .Handle(new CancelBookingCommand { BookingId = booking.Id }, CancellationToken.None));

        Assert.Equal("BOOKING_FORBIDDEN", ex.Code);
    }
}
