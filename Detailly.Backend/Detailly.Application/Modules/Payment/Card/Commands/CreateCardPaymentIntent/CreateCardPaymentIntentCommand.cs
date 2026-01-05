
using MediatR;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;

public record CreateCardPaymentIntentCommand(
    int BookingId,
    int UserId
) : IRequest<CreateCardPaymentIntentResult>;
