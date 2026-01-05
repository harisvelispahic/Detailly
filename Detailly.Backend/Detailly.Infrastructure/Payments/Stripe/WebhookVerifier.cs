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

        // Stripe signature format:
        // t=timestamp,v1=hash
        var parts = signatureHeader.Split(',');

        var v1 = parts
            .FirstOrDefault(p => p.StartsWith("v1="))
            ?.Split('=')[1];

        if (v1 is null)
            return false;

        // Compute HMAC: HMAC_SHA256(secret, payload)
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

        var expected = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return expected == v1;
    }
}
