namespace Detailly.Infrastructure.Database.Configurations.Identity;

public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ApplicationUserId, x.TokenHash })
            .IsUnique();

        builder.Property(x => x.TokenHash)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.IsRevoked)
            .HasDefaultValue(false);

        builder.Property(x => x.Fingerprint)
            .HasMaxLength(200);
    }
}
