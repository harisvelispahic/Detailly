using Detailly.Application.Abstractions;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database;

public partial class DatabaseContext : DbContext, IAppDbContext
{
    // Identity
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<MarketUserEntity> Users => Set<MarketUserEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    // Other tables
    public DbSet<BookingEntity> Bookings => Set<BookingEntity>();
    public DbSet<BookingEntity> Reviews => Set<BookingEntity>();
    public DbSet<ServicePackageEntity> ServicePackages => Set<ServicePackageEntity>();
    public DbSet<ServicePackageItemEntity> ServicePackageItems => Set<ServicePackageItemEntity>();
    public DbSet<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments => Set<ServicePackageItemAssignmentEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<TimeSlotEntity> TimeSlots => Set<TimeSlotEntity>();





    private readonly TimeProvider _clock;
    public DatabaseContext(DbContextOptions<DatabaseContext> options, TimeProvider clock) : base(options)
    {
        _clock = clock;
    }
}