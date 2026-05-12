namespace Detailly.Application.Modules.Settings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSystemSettingsQuery, SystemSettingsDto>
{
    public async Task<SystemSettingsDto> Handle(GetSystemSettingsQuery request, CancellationToken ct)
    {
        var settings = await context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(ct)
            ?? throw new DetaillyNotFoundException("System settings not found.");

        return new SystemSettingsDto
        {
            StandardWalletBonusPercent = settings.StandardWalletBonusPercent,
            FleetWalletBonusPercent = settings.FleetWalletBonusPercent,
            ReviewWindowDays = settings.ReviewWindowDays,
            BaseFleetDiscountPercent = settings.BaseFleetDiscountPercent,
            PerVehicleFleetDiscountPercent = settings.PerVehicleFleetDiscountPercent,
            MaxFleetDiscountPercent = settings.MaxFleetDiscountPercent,
        };
    }
}
