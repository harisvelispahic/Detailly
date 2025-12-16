using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using Detailly.Domain.Entities.Vehicle;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Detailly.Application.Abstractions;

// Application layer
public interface IAppDbContext
{
    // reference to the Database data type, for transactions
    DatabaseFacade Database { get; }



    // Identity
    DbSet<ApplicationUserEntity> ApplicationUsers { get; }
    DbSet<RefreshTokenEntity> RefreshTokens { get; }

    // Sales
    DbSet<OrderEntity> Orders { get; }
    DbSet<OrderItemEntity> OrderItems { get; }
    DbSet<CartEntity> Carts { get; }
    DbSet<CartItemEntity> CartItems { get; }

    // Catalog
    DbSet<ProductCategoryEntity> ProductCategories { get; }
    DbSet<ProductEntity> Products { get; }
    DbSet<InventoryEntity> Inventory { get; }

    // Booking
    DbSet<BookingEntity> Bookings { get; }
    DbSet<ReviewEntity> Reviews { get; }
    DbSet<ServicePackageEntity> ServicePackages { get; }
    DbSet<ServicePackageItemEntity> ServicePackageItems { get; }
    DbSet<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; }
    DbSet<LocationEntity> Locations { get; }
    DbSet<TimeSlotEntity> TimeSlots { get; }

    DbSet<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; }

    // Vehicle
    DbSet<VehicleEntity> Vehicles { get; }
    DbSet<VehicleCategoryEntity> VehicleCategories { get; }

    // Payment
    DbSet<WalletEntity> Wallet { get; }
    DbSet<PaymentTransactionEntity> PaymentTransactions { get; }

    // Shared
    DbSet<AddressEntity> Addresses { get; }
    DbSet<ImageEntity> Images { get; }
    DbSet<NotificationEntity> Notifications { get; }


    Task<int> SaveChangesAsync(CancellationToken ct);
}