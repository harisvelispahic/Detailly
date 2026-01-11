
using MediatR;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;

public record CreateCardPaymentIntentCommand(
    int UserId,
    int BookingId
) : IRequest<CreateCardPaymentIntentResult>;
