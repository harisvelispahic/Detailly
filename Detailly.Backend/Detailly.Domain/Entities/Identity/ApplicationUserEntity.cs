// ApplicationUserEntity.cs
using Detailly.Domain.Common;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Domain.Entities.Identity;

public sealed class ApplicationUserEntity : BaseEntity
{    
    public bool IsManager { get; set; }
    public int TokenVersion { get; set; } = 0;  // For global revocation
    public bool IsEnabled { get; set; }


    public int WalletId { get; set; }
    public WalletEntity? Wallet { get; set; }
    public int AddressId { get; set; }
    public AddressEntity? Address { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Phone { get; set; }
    public bool IsFleet { get; set; }
    public bool IsEmployee { get; set; }
    public bool IsAdmin { get; set; }
    public string? CompanyName { get; set; }

    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}