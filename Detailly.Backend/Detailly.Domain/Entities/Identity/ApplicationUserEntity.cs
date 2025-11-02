using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Domain.Entities.Identity;

public sealed class ApplicationUserEntity : BaseEntity
{    
    public bool IsManager { get; set; }
    public int TokenVersion { get; set; } = 0;  // For global revocation
    public bool IsEnabled { get; set; }


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

    // Foreign keys
    public WalletEntity? Wallet { get; set; }
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
    public CartEntity? Cart { get; set; }
    public IReadOnlyCollection<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();
    public IReadOnlyCollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    public IReadOnlyCollection<VehicleEntity> Vehicles { get; private set; } = new List<VehicleEntity>();
    public IReadOnlyCollection<NotificationEntity> Notifications { get; private set; } = new List<NotificationEntity>();

    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
}