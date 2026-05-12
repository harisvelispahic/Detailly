namespace Detailly.Application.Modules.Settings.Commands.UpdateSystemSettings;

public class UpdateSystemSettingsCommandValidator : AbstractValidator<UpdateSystemSettingsCommand>
{
    public UpdateSystemSettingsCommandValidator()
    {
        RuleFor(x => x.StandardWalletBonusPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.FleetWalletBonusPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.ReviewWindowDays).InclusiveBetween(1, 90);
        RuleFor(x => x.BaseFleetDiscountPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.PerVehicleFleetDiscountPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.MaxFleetDiscountPercent).InclusiveBetween(0, 100);
    }
}
