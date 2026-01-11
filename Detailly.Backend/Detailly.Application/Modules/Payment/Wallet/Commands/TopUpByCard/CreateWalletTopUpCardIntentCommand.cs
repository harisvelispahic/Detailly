using MediatR;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUpByCard;

public record CreateWalletTopUpCardIntentCommand(
    int UserId,
    decimal Amount,
    string? Description
) : IRequest<CreateWalletTopUpCardIntentResult>;

public record CreateWalletTopUpCardIntentResult
{
    public required string ClientSecret { get; init; }
}
