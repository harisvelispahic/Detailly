using Detailly.Application.Common.Exceptions;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Tests.WalletTopUpTests.UnitTests;

public class WalletTopUpUnitTests
{
    // ---- Helpers ----

    private static DatabaseContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var clock = new Microsoft.Extensions.Time.Testing.FakeTimeProvider();
        return new DatabaseContext(options, clock);
    }

    private static async Task<WalletEntity> CreateWalletAsync(
        DatabaseContext context,
        int userId = 1,
        decimal balance = 0m,
        decimal totalDeposited = 0m,
        int percentageAdded = 10)
    {
        var wallet = new WalletEntity
        {
            ApplicationUserId = userId,
            Balance = balance,
            TotalDeposited = totalDeposited,
            PercentageAdded = percentageAdded,
        };
        context.Wallet.Add(wallet);
        await context.SaveChangesAsync(CancellationToken.None);
        return wallet;
    }

    // ========================
    // Balance and bonus math
    // ========================

    [Fact]
    public async Task Handle_ShouldAddAmountPlusBonus_ToBalance()
    {
        // 10% bonus on 100 m → Balance = 110 m
        using var context = GetInMemoryDbContext();
        var wallet = await CreateWalletAsync(context, percentageAdded: 10);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 100m, null), CancellationToken.None);

        Assert.Equal(110m, wallet.Balance);
    }

    [Fact]
    public async Task Handle_ShouldOnlyTrackBaseAmount_InTotalDeposited()
    {
        // TotalDeposited must not include the bonus
        using var context = GetInMemoryDbContext();
        var wallet = await CreateWalletAsync(context, percentageAdded: 10);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 100m, null), CancellationToken.None);

        Assert.Equal(100m, wallet.TotalDeposited);
    }

    [Fact]
    public async Task Handle_ShouldNotAddBonus_WhenPercentageIsZero()
    {
        using var context = GetInMemoryDbContext();
        var wallet = await CreateWalletAsync(context, percentageAdded: 0);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 150m, null), CancellationToken.None);

        Assert.Equal(150m, wallet.Balance);
        Assert.Equal(150m, wallet.TotalDeposited);
    }

    [Fact]
    public async Task Handle_ShouldCalculateBonus_WithCustomPercentage()
    {
        // 20% bonus on 200 m → 200 + 40 = 240 m balance, 200 m deposited
        using var context = GetInMemoryDbContext();
        var wallet = await CreateWalletAsync(context, percentageAdded: 20);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 200m, null), CancellationToken.None);

        Assert.Equal(240m, wallet.Balance);
        Assert.Equal(200m, wallet.TotalDeposited);
    }

    [Fact]
    public async Task Handle_ShouldAccumulateCorrectly_OnExistingBalance()
    {
        // Pre-existing: 50 m balance, 40 m deposited; top up 100 m at 10%
        // Expected: balance = 50 + 100 + 10 = 160 m, deposited = 40 + 100 = 140 m
        using var context = GetInMemoryDbContext();
        var wallet = await CreateWalletAsync(context, balance: 50m, totalDeposited: 40m, percentageAdded: 10);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 100m, null), CancellationToken.None);

        Assert.Equal(160m, wallet.Balance);
        Assert.Equal(140m, wallet.TotalDeposited);
    }

    // ========================
    // Guard / error cases
    // ========================

    [Fact]
    public async Task Handle_ShouldThrow_WhenAmountIsZero()
    {
        using var context = GetInMemoryDbContext();
        await CreateWalletAsync(context);
        var handler = new TopUpWalletCommandHandler(context);

        var ex = await Assert.ThrowsAsync<DetaillyBusinessRuleException>(
            () => handler.Handle(new TopUpWalletCommand(1, 0m, null), CancellationToken.None));

        Assert.Equal("TOPUP_INVALID_AMOUNT", ex.Code);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAmountIsNegative()
    {
        using var context = GetInMemoryDbContext();
        await CreateWalletAsync(context);
        var handler = new TopUpWalletCommandHandler(context);

        var ex = await Assert.ThrowsAsync<DetaillyBusinessRuleException>(
            () => handler.Handle(new TopUpWalletCommand(1, -50m, null), CancellationToken.None));

        Assert.Equal("TOPUP_INVALID_AMOUNT", ex.Code);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenWalletNotFound()
    {
        using var context = GetInMemoryDbContext();
        // No wallet seeded; UserId 99 does not exist
        var handler = new TopUpWalletCommandHandler(context);

        await Assert.ThrowsAsync<DetaillyNotFoundException>(
            () => handler.Handle(new TopUpWalletCommand(99, 100m, null), CancellationToken.None));
    }

    // ========================
    // Transaction record
    // ========================

    [Fact]
    public async Task Handle_ShouldCreateDepositTransaction_WithCorrectFields()
    {
        using var context = GetInMemoryDbContext();
        await CreateWalletAsync(context);
        var handler = new TopUpWalletCommandHandler(context);

        await handler.Handle(new TopUpWalletCommand(1, 100m, "Monthly top-up"), CancellationToken.None);

        var tx = context.PaymentTransactions.Single();
        Assert.Equal(100m, tx.Amount);
        Assert.Equal(TransactionType.Deposit, tx.TransactionType);
        Assert.Equal(PaymentTransactionStatus.Paid, tx.Status);
        Assert.Equal("Monthly top-up", tx.Description);
    }
}
