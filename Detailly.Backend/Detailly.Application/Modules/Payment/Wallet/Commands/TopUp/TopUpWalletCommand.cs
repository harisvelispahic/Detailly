public record TopUpWalletCommand(
    int UserId,
    decimal Amount,
    string? Description
) : IRequest<Unit>;
