using Detailly.Application.Abstractions.PDF;
using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.ExportShiftsPdf;

public sealed class ExportShiftsPdfCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IShiftsPdfGenerator pdfGenerator)
    : IRequestHandler<ExportShiftsPdfCommand, byte[]>
{
    public async Task<byte[]> Handle(ExportShiftsPdfCommand request, CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

        var start = request.StartDateUtc.Date;
        var end = request.EndDateUtc.Date.AddDays(1);

        var query = context.EmployeeShifts
            .AsNoTracking()
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.StartUtc >= start &&
                s.StartUtc < end);

        if (request.EmployeeWorkMode is not null)
            query = query.Where(s => s.EmployeeWorkMode == request.EmployeeWorkMode.Value);

        var shifts = await query
            .OrderBy(s => s.StartUtc)
            .ThenBy(s => s.Employee.FirstName)
            .Select(s => new ExportShiftsQueryDto
            {
                Id = s.Id,
                EmployeeName = s.Employee.FirstName + " " + s.Employee.LastName,
                EmployeeWorkMode = s.EmployeeWorkMode,
                StartUtc = s.StartUtc,
                EndUtc = s.EndUtc,
                LocationName = s.ShopLocation.Name,
            })
            .ToListAsync(ct);

        var locationName = shifts.FirstOrDefault()?.LocationName ?? string.Empty;
        return pdfGenerator.Generate(shifts, request.StartDateUtc, request.EndDateUtc, locationName);
    }
}
