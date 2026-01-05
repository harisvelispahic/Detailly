public record PayBookingWithWalletCommand(
    int UserId,
    int BookingId
) : IRequest<Unit>;
