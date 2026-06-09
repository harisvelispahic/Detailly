using Detailly.Application.Abstractions.PDF;
using Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.ExportMyBookingsPdf;

public sealed class ExportMyBookingsPdfCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IBookingsPdfGenerator pdfGenerator)
    : IRequestHandler<ExportMyBookingsPdfCommand, byte[]>
{
    public async Task<byte[]> Handle(ExportMyBookingsPdfCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var start = request.StartDateUtc.Date;
        var end = request.EndDateUtc.Date.AddDays(1);

        var bookings = await context.Bookings
            .AsNoTracking()
            .Where(b =>
                !b.IsDeleted &&
                b.CustomerId == userId &&
                b.StartUtc >= start &&
                b.StartUtc < end &&
                b.Status != BookingStatus.Draft)
            .OrderBy(b => b.StartUtc)
            .Select(b => new ExportMyBookingsQueryDto
            {
                Id = b.Id,
                Status = b.Status,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                TotalPrice = b.TotalPrice,
                ServicePackageName = b.ServicePackage.Name,
            })
            .ToListAsync(ct);

        return pdfGenerator.Generate(bookings, request.StartDateUtc, request.EndDateUtc, request.CustomerName);
    }
}
