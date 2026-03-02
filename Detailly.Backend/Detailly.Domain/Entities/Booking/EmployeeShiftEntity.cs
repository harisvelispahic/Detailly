using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Booking;

public class EmployeeShiftEntity : BaseEntity
{
    public required int EmployeeId { get; set; }
    public ApplicationUserEntity Employee { get; set; } = null!;

    // InShop shifts belong to the shop location.
    // For Mobile shifts, you can keep ShopLocationId as the same shop (home base).
    public required int ShopLocationId { get; set; }
    public LocationEntity ShopLocation { get; set; } = null!;

    public required EmployeeWorkMode EmployeeWorkMode { get; set; }

    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}