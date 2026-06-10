using Detailly.Domain.Entities.Payment;

namespace Detailly.Infrastructure.Database.Configurations.Payment;

public sealed class WalletConfiguration
    : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalDeposited)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasOne(x => x.ApplicationUser)
            .WithOne(x => x.Wallet)
            .HasForeignKey<WalletEntity>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
