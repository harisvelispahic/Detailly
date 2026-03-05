namespace Detailly.Infrastructure.Database.Configurations.Identity;

public sealed class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUserEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationUserEntity> builder)
    {
        builder.ToTable("ApplicationUsers");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.PasswordHash).IsRequired();

        builder.Property(x => x.IsAdmin).HasDefaultValue(false);
        builder.Property(x => x.IsManager).HasDefaultValue(false);
        builder.Property(x => x.IsEmployee).HasDefaultValue(false);

        builder.Property(x => x.TokenVersion).HasDefaultValue(0);
        builder.Property(x => x.IsEnabled).HasDefaultValue(true);

        // Customer → Bookings (now CustomerId)
        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Refresh tokens
        builder.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey(x => x.ApplicationUserId);

        // ✅ Address book relationship is configured from AddressConfiguration (below)

        // Index for employee availability filtering
        builder.HasIndex(x => new { x.IsEmployee, x.EmployeeWorkMode, x.IsEnabled });
    }
}