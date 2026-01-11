using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundPayment;
public class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, Unit>
{
    private readonly IAppDbContext _context;

    public RefundPaymentCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RefundPaymentCommand request, CancellationToken ct)
    {
        var payment = await _context.PaymentTransactions
            .Include(x => x.Wallet)
            .Include(x => x.Booking)
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId, ct)
            ?? throw new Exception("Payment not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new Exception("Only paid transactions can be refunded.");

        payment.Status = PaymentTransactionStatus.Refunded;

        if (payment.Wallet is not null)
            payment.Wallet.Balance += payment.Amount;

        if (payment.Booking is not null)
            payment.Booking.Status = BookingStatus.Cancelled;

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
