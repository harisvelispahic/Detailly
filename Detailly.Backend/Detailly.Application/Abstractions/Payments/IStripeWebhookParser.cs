
namespace Detailly.Application.Abstractions.Payments;

public interface IStripeWebhookParser
{
    (string EventId, string EventType, string PaymentIntentId)? Parse(string payload);
}
