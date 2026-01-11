
namespace Detailly.Application.Abstractions.Payments;

public interface IWebhookVerifier
{
    bool Verify(string payload, string signatureHeader, string webhookSecret);
}
