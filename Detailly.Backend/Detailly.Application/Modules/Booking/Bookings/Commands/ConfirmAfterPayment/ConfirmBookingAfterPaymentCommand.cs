using MediatR;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;

public sealed record ConfirmBookingAfterPaymentCommand(int PaymentTransactionId) : IRequest<Unit>;