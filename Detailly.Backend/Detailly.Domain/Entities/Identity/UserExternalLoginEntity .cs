using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Identity;
public sealed class UserExternalLoginEntity : BaseEntity
{
    public int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;

    public required string Provider { get; set; }        // "Google"
    public required string ProviderUserId { get; set; }  // "sub"
    public string? Email { get; set; }
}