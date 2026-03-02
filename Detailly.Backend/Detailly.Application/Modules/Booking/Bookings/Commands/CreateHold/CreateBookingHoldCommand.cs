using Detailly.Domain.Common.Enums;
using MediatR;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;

public sealed class CreateBookingHoldCommand : IRequest<int>
{
    public required int ServicePackageId { get; set; }
    public List<int>? AddonItemIds { get; set; }

    public required ServiceMode ServiceMode { get; set; }
    public required int ShopLocationId { get; set; }

    public int? ServiceAddressId { get; set; } // required when Mobile

    public required DateTime StartUtc { get; set; }

    public List<int>? VehicleIds { get; set; }
    public string? Notes { get; set; }
}