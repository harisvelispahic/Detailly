using Detailly.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Shared;

public sealed class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettingsEntity>
{
    public void Configure(EntityTypeBuilder<SystemSettingsEntity> builder)
    {
        builder.ToTable("SystemSettings");

        builder.Property(x => x.StandardWalletBonusPercent).IsRequired();
        builder.Property(x => x.FleetWalletBonusPercent).IsRequired();
        builder.Property(x => x.ReviewWindowDays).IsRequired();
        builder.Property(x => x.BaseFleetDiscountPercent).IsRequired();
        builder.Property(x => x.PerVehicleFleetDiscountPercent).IsRequired();
        builder.Property(x => x.MaxFleetDiscountPercent).IsRequired();

        // Seed the single singleton row
        builder.HasData(new SystemSettingsEntity
        {
            Id = 1,
            StandardWalletBonusPercent = 10,
            FleetWalletBonusPercent = 15,
            ReviewWindowDays = 7,
            BaseFleetDiscountPercent = 2.0m,
            PerVehicleFleetDiscountPercent = 1.0m,
            MaxFleetDiscountPercent = 8.0m,
            IsDeleted = false,
            CreatedAtUtc = new DateTime(2026, 5, 12, 0, 0, 0, DateTimeKind.Utc),
            ModifiedAtUtc = null,
        });
    }
}
