using Detailly.Application.Abstractions.Payments;
using Stripe;

namespace Detailly.Infrastructure.Payments.Stripe;

public class StripeWebhookParser : IStripeWebhookParser
{
    public (string EventId, string EventType, string PaymentIntentId)? Parse(string payload, string signatureHeader, string webhookSecret)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, webhookSecret);
        }
        catch (StripeException)
        {
            return null;
        }

        if (stripeEvent.Data?.Object is not PaymentIntent intent)
            return null;

        return (stripeEvent.Id, stripeEvent.Type, intent.Id);
    }
}
