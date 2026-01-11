using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;
public class PayBookingWithWalletCommandHandler
    : IRequestHandler<PayBookingWithWalletCommand, Unit>
{
    private readonly IAppDbContext _context;

    public PayBookingWithWalletCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(PayBookingWithWalletCommand request, CancellationToken ct)
    {
        var booking = await _context.Bookings
            .Include(x => x.PaymentTransaction)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, ct)
            ?? throw new Exception("Booking not found.");

        if (booking.ApplicationUserId != request.UserId)
            throw new Exception("Forbidden.");

        if (booking.Status != BookingStatus.PendingPayment)
            throw new Exception("Booking is not awaiting payment.");

        var wallet = await _context.Wallet
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId, ct)
            ?? throw new Exception("Wallet not found.");

        if (wallet.Balance < booking.TotalPrice)
            throw new Exception("Insufficient wallet balance.");

        wallet.Balance -= booking.TotalPrice;

        var transaction = new PaymentTransactionEntity
        {
            Amount = booking.TotalPrice,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Paid,
            Wallet = wallet,
            Booking = booking,
            TransactionDate = DateTime.UtcNow,
            Provider = "Wallet",
            Description = "Booking paid with wallet"
        };

        _context.PaymentTransactions.Add(transaction);

        booking.PaymentTransaction = transaction;
        booking.Status = BookingStatus.Confirmed;

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
