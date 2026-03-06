namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetById;

public sealed class GetBookingByIdQuery : IRequest<GetBookingByIdQueryDto>
{
    public required int Id { get; set; }
}