
namespace Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;
public record PayBookingWithWalletCommand(
    int UserId,
    int BookingId
) : IRequest<Unit>;
