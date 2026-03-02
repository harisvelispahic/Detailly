
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;

public sealed class ListBookingsForDateQueryDto
{
    public int Id { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public int RequiredEmployees { get; set; }
    public int RequiredBays { get; set; }

    public string CustomerName { get; set; } = string.Empty;
    public string ServicePackageName { get; set; } = string.Empty;

    public string? Notes { get; set; }

    // Useful if IncludePendingPaymentHolds = true
    public DateTime? ReservationExpiresAtUtc { get; set; }

    public List<ListBookingsForDateAssignedEmployeeDto> AssignedEmployees { get; set; } = new();
}

public sealed class ListBookingsForDateAssignedEmployeeDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
}