namespace Detailly.Application.Modules.Settings.Commands.UpdateSystemSettings;

public class UpdateSystemSettingsCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateSystemSettingsCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSystemSettingsCommand request, CancellationToken ct)
    {
        var settings = await context.SystemSettings
            .FirstOrDefaultAsync(ct)
            ?? throw new DetaillyNotFoundException("System settings not found.");

        settings.StandardWalletBonusPercent = request.StandardWalletBonusPercent;
        settings.FleetWalletBonusPercent = request.FleetWalletBonusPercent;
        settings.ReviewWindowDays = request.ReviewWindowDays;
        settings.BaseFleetDiscountPercent = request.BaseFleetDiscountPercent;
        settings.PerVehicleFleetDiscountPercent = request.PerVehicleFleetDiscountPercent;
        settings.MaxFleetDiscountPercent = request.MaxFleetDiscountPercent;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
