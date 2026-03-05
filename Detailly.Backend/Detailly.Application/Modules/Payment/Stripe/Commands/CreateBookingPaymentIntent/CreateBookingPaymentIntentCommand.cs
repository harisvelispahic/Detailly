namespace Detailly.Application.Modules.Payment.Card.Commands.CreateBookingPaymentIntent;

public record CreateBookingPaymentIntentCommand(
    int UserId,
    int BookingId
) : IRequest<CreateBookingPaymentIntentResult>;
