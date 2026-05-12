namespace Detailly.Application.Modules.Settings;

public class SystemSettingsDto
{
    public int StandardWalletBonusPercent { get; set; }
    public int FleetWalletBonusPercent { get; set; }
    public int ReviewWindowDays { get; set; }
    public decimal BaseFleetDiscountPercent { get; set; }
    public decimal PerVehicleFleetDiscountPercent { get; set; }
    public decimal MaxFleetDiscountPercent { get; set; }
}
