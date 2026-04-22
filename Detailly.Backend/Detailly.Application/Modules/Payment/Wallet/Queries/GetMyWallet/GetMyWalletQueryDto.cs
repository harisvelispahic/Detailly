namespace Detailly.Application.Modules.Payment.Wallet.Queries.GetMyWallet;

public class GetMyWalletQueryDto
{
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal TotalDeposited { get; init; }
    public int PercentageAdded { get; init; }
}
