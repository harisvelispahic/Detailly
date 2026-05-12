using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Shared;

public class SystemSettingsEntity : BaseEntity
{
    // Wallet top-up bonus (%)
    public int StandardWalletBonusPercent { get; set; } = 10;
    public int FleetWalletBonusPercent { get; set; } = 15;

    // Review submission window after booking completion
    public int ReviewWindowDays { get; set; } = 7;

    // Fleet booking discount
    public decimal BaseFleetDiscountPercent { get; set; } = 2.0m;
    public decimal PerVehicleFleetDiscountPercent { get; set; } = 1.0m;
    public decimal MaxFleetDiscountPercent { get; set; } = 8.0m;
}
