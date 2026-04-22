namespace Detailly.Application.Modules.Payment.Wallet.Queries.GetMyWallet;

public class GetMyWalletQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyWalletQuery, GetMyWalletQueryDto>
{
    public async Task<GetMyWalletQueryDto> Handle(GetMyWalletQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var dto = await context.Wallet
            .AsNoTracking()
            .Where(w => w.ApplicationUserId == currentUser.ApplicationUserId.Value)
            .Select(w => new GetMyWalletQueryDto
            {
                Balance = w.Balance,
                Currency = w.Currency.ToString(),
                TotalDeposited = w.TotalDeposited,
                PercentageAdded = w.PercentageAdded
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new DetaillyNotFoundException("Wallet not found.");

        return dto;
    }
}
