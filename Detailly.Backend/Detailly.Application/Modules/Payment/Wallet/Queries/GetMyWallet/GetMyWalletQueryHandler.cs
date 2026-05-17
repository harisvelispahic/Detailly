namespace Detailly.Application.Modules.Payment.Wallet.Queries.GetMyWallet;

public class GetMyWalletQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyWalletQuery, GetMyWalletQueryDto>
{
    public async Task<GetMyWalletQueryDto> Handle(GetMyWalletQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var wallet = await context.Wallet
            .AsNoTracking()
            .Include(w => w.ApplicationUser)
            .Where(w => w.ApplicationUserId == currentUser.ApplicationUserId.Value)
            .FirstOrDefaultAsync(ct);

        if (wallet is null)
            throw new DetaillyNotFoundException("Wallet not found.");

        var settings = await context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(ct);

        var bonusPercent = settings is null
            ? wallet.PercentageAdded
            : (wallet.ApplicationUser!.IsFleet
                ? settings.FleetWalletBonusPercent
                : settings.StandardWalletBonusPercent);

        return new GetMyWalletQueryDto
        {
            Balance = wallet.Balance,
            Currency = wallet.Currency.ToString(),
            TotalDeposited = wallet.TotalDeposited,
            PercentageAdded = bonusPercent
        };
    }
}
