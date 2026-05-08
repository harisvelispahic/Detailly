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
    public DbSet<UserExternalLoginEntity> UserExternalLogins => Set<UserExternalLoginEntity>();



    // Sales
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
 
    public DbSet<CartEntity> Carts => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();
    public DbSet<SavedProductEntity> SavedProducts => Set<SavedProductEntity>();



    // Catalog
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<InventoryEntity> Inventory => Set<InventoryEntity>();



    // Booking
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<ReviewEntity> Reviews => Set<ReviewEntity>();
    public DbSet<ReactionEntity> Reactions => Set<ReactionEntity>();
    public DbSet<ServicePackageEntity> ServicePackages => Set<ServicePackageEntity>();
    public DbSet<ServicePackageItemEntity> ServicePackageItems => Set<ServicePackageItemEntity>();
    public DbSet<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments => Set<ServicePackageItemAssignmentEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<BookingItemEntity> BookingItems => Set<BookingItemEntity>();
    public DbSet<EmployeeShiftEntity> EmployeeShifts => Set<EmployeeShiftEntity>();
    public DbSet<BookingEmployeeAssignmentEntity> BookingEmployeeAssignments => Set<BookingEmployeeAssignmentEntity>();
    public DbSet<BookingVehicleAssignmentEntity> BookingVehicleAssignments => Set<BookingVehicleAssignmentEntity>();
    public DbSet<LocationOpeningHoursEntity> LocationOpeningHours => Set<LocationOpeningHoursEntity>();



    // Vehicle
    public DbSet< VehicleEntity> Vehicles => Set<VehicleEntity>();
    public DbSet<VehicleCategoryEntity> VehicleCategories => Set<VehicleCategoryEntity>();



    // Payment
    public DbSet<WalletEntity> Wallet => Set<WalletEntity>();
    public DbSet<PaymentTransactionEntity> PaymentTransactions => Set<PaymentTransactionEntity>();
    public DbSet<ProcessedWebhookEventEntity> ProcessedWebhookEvents => Set<ProcessedWebhookEventEntity>();



    // Shared
    public DbSet<AddressEntity> Addresses => Set<AddressEntity>();
    public DbSet<ImageEntity> Images => Set<ImageEntity>();



    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}