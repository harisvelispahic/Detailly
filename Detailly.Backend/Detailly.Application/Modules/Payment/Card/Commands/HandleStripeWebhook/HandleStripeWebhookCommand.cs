
using MediatR;

namespace Detailly.Application.Modules.Payment.Card.Commands.HandleStripeWebhook;

public record HandleStripeWebhookCommand(
    string Payload,
    string? SignatureHeader
) : IRequest<Unit>;
