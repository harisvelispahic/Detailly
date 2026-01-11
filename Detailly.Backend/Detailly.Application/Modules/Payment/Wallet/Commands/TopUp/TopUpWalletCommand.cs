
namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;
public record TopUpWalletCommand(
    int UserId,
    decimal Amount,
    string? Description
) : IRequest<Unit>;
