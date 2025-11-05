using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Infrastructure.Database.Seeders;
using System.Linq.Expressions;

namespace Detailly.Infrastructure.Database;

public partial class DatabaseContext
{
    private DateTime UtcNow => _clock.GetUtcNow().UtcDateTime;

    private void ApplyAuditAndSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = UtcNow;
                    entry.Entity.ModifiedAtUtc = null; // ili = UtcNow
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAtUtc = UtcNow;
                    break;

                case EntityState.Deleted:
                    // soft-delete: set is Modified and IsDeleted
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.ModifiedAtUtc = UtcNow;
                    break;
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<decimal?>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- ServicePackageItemAssignment (N:M medjutabela) ----------
        modelBuilder.Entity<ServicePackageItemAssignmentEntity>(entity =>
        {
            entity.ToTable("ServicePackageItemAssignments"); // ime tabele

            entity.HasKey(e => e.Id); // surogat ključ

            entity.HasOne(e => e.ServicePackage)
                  .WithMany(p => p.ServicePackageItemAssignments)
                  .HasForeignKey(e => e.ServicePackageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ServicePackageItem)
                  .WithMany(i => i.ServicePackageItemAssignments)
                  .HasForeignKey(e => e.ServicePackageItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Booking - Review (1:1, shared PK) ----------
        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.ToTable("Bookings"); // ime tabele

            entity.HasOne(b => b.Review)
                  .WithOne(r => r.Booking)
                  .HasForeignKey<ReviewEntity>(r => r.BookingId) // shared PK i FK
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReviewEntity>(entity =>
        {
            entity.ToTable("Reviews"); // ime tabele
            entity.HasKey(r => r.BookingId); // shared PK
        });

        // dodano
        modelBuilder.Entity<BookingVehicleAssignmentEntity>()
        .HasOne(bva => bva.Vehicle)
        .WithMany(v => (IEnumerable<BookingVehicleAssignmentEntity>)v.BookingVehicleAssignments)
        .HasForeignKey(bva => bva.VehicleId)
        .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

        modelBuilder.Entity<BookingVehicleAssignmentEntity>()
            .HasOne(bva => bva.Booking)
            .WithMany(b => (IEnumerable<BookingVehicleAssignmentEntity>)b.BookingVehicleAssignments)
            .HasForeignKey(bva => bva.BookingId)
            .OnDelete(DeleteBehavior.Restrict); // also optional
        // do ovdje

        // ---------- Decimal precision ----------
        //modelBuilder.Properties<decimal>().HavePrecision(18, 2);
        //modelBuilder.Properties<decimal?>().HavePrecision(18, 2);

        // ---------- Global soft-delete filter ----------
        ApplyGlobalFilters(modelBuilder);

        // ---------- Static seed data ----------
        StaticDataSeeder.Seed(modelBuilder);
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        // Apply a global filter to all entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var compare = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(lambda);
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyAuditAndSoftDelete();

        return base.SaveChangesAsync(cancellationToken);
    }
}