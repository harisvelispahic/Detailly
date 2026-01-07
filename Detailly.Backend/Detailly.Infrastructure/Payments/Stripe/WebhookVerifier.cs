using System.Security.Cryptography;
using System.Text;
using Detailly.Application.Abstractions.Payments;

namespace Detailly.Infrastructure.Payments.Stripe;

public class WebhookVerifier : IWebhookVerifier
{
    public bool Verify(string payload, string signatureHeader, string webhookSecret)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader))
            return false;

        // Parse signature header:
        // t=timestamp,v1=hash
        var parts = signatureHeader.Split(',');

        var timestampPart = parts.FirstOrDefault(p => p.StartsWith("t="));
        var v1Part = parts.FirstOrDefault(p => p.StartsWith("v1="));

        if (timestampPart is null || v1Part is null)
            return false;

        var timestamp = timestampPart.Split('=')[1];
        var v1 = v1Part.Split('=')[1];

        // Construct signed payload (THIS is the key!)
        var signedPayload = $"{timestamp}.{payload}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));

        var expected = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return expected == v1;
    }

}
