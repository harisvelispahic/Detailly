
using Detailly.Domain.Common;
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


        // Include all entity configuration files from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

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
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Apply filter to any entity that has a bool IsDeleted property
            var isDeletedProp = clrType.GetProperty(nameof(BaseEntity.IsDeleted));
            if (isDeletedProp is null || isDeletedProp.PropertyType != typeof(bool))
                continue;

            var parameter = Expression.Parameter(clrType, "e");

            // EF.Property<bool>(e, "IsDeleted") so it works even if it isn't BaseEntity
            var efPropertyMethod = typeof(EF).GetMethod(nameof(EF.Property))!
                .MakeGenericMethod(typeof(bool));

            var isDeletedPropertyAccess = Expression.Call(
                efPropertyMethod,
                parameter,
                Expression.Constant(nameof(BaseEntity.IsDeleted)));

            var compare = Expression.Equal(isDeletedPropertyAccess, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
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