using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Identity;

public sealed class RefreshTokenEntity : BaseEntity
{
    public string TokenHash { get; set; } // Store the HASH, not the plain token
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public string? Fingerprint { get; set; } // (Optional) e.g., UA/IP hash
    public DateTime? RevokedAtUtc { get; set; }

    // Foreign keys
    public int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = default!;
}