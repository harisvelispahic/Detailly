using Detailly.Application.Abstractions;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    // Identity
    public DbSet<ApplicationUserEntity> ApplicationUsers => Set<ApplicationUserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    // Sales
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemAssignmentEntity> OrderItemAssignments => Set<OrderItemAssignmentEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<OrderStatusEntity> OrderStatus => Set<OrderStatusEntity>();
    public DbSet<CartEntity> Cart => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();

    // Catalog
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<InventoryEntity> Inventory => Set<InventoryEntity>();

    // Booking
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<BookingEntity> Reviews => Set<BookingEntity>();
    public DbSet<ServicePackageEntity> ServicePackages => Set<ServicePackageEntity>();
    public DbSet<ServicePackageItemEntity> ServicePackageItems => Set<ServicePackageItemEntity>();
    public DbSet<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments => Set<ServicePackageItemAssignmentEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<TimeSlotEntity> TimeSlots => Set<TimeSlotEntity>();

    public DbSet<BookingVehicleAssignmentEntity> BookingVehicleAssignments => Set<BookingVehicleAssignmentEntity>();

    // Vehicle
    public DbSet< VehicleEntity> Vehicles => Set<VehicleEntity>();
    public DbSet<VehicleCategoryEntity> VehicleCategories => Set<VehicleCategoryEntity>();

    // Payment
    public DbSet<WalletEntity> Wallet => Set<WalletEntity>();
    public DbSet<PaymentTransactionEntity> PaymentTransactions => Set<PaymentTransactionEntity>();
    public DbSet<PaymentTransactionStatusEntity> PaymentTransactionStatus => Set<PaymentTransactionStatusEntity>();

    // Shared
    public DbSet<AddressEntity> Address => Set<AddressEntity>();
    public DbSet<ImageEntity> Images => Set<ImageEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();


    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}