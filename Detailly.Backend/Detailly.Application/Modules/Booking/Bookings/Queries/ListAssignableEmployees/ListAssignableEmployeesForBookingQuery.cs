namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListAssignableEmployees;

public sealed record ListAssignableEmployeesForBookingQuery(int BookingId) 
    : IRequest<List<ListAssignableEmployeesForBookingQueryDto>>;