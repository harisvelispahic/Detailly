using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;
using System.ComponentModel.DataAnnotations;

namespace Detailly.Domain.Entities.Identity;

public sealed class ApplicationUserEntity : BaseEntity
{
    public int TokenVersion { get; set; } = 0;  // For global revocation

    public bool IsManager { get; set; } = false;
    public bool IsEnabled { get; set; } = true;
    public required bool IsFleet { get; set; }      // user defined
    public bool IsAdmin { get; set; } = false;
    public bool IsEmployee { get; set; } = false;


    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }

    [MaxLength(200)]
    [EmailAddress]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }

    // Foreign keys
    public WalletEntity? Wallet { get; set; }
    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
    public CartEntity? Cart { get; set; }
    public ImageEntity? Image { get; set; }
    public IReadOnlyCollection<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();
    public IReadOnlyCollection<OrderEntity> Orders { get; private set; } = new List<OrderEntity>();
    public IReadOnlyCollection<VehicleEntity> Vehicles { get; private set; } = new List<VehicleEntity>();
    public IReadOnlyCollection<NotificationEntity> Notifications { get; private set; } = new List<NotificationEntity>();
    //public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();

    public ICollection<RefreshTokenEntity> RefreshTokens { get; private set; } = new List<RefreshTokenEntity>();
    public ICollection<UserExternalLoginEntity> ExternalLogins { get; private set; } = new List<UserExternalLoginEntity>();
}