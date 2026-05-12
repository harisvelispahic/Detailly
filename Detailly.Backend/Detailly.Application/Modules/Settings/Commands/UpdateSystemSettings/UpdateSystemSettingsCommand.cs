namespace Detailly.Application.Modules.Settings.Commands.UpdateSystemSettings;

public class UpdateSystemSettingsCommand : IRequest<Unit>
{
    public int StandardWalletBonusPercent { get; set; }
    public int FleetWalletBonusPercent { get; set; }
    public int ReviewWindowDays { get; set; }
    public decimal BaseFleetDiscountPercent { get; set; }
    public decimal PerVehicleFleetDiscountPercent { get; set; }
    public decimal MaxFleetDiscountPercent { get; set; }
}
