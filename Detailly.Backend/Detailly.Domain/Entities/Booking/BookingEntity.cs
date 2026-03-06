using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking;

public class BookingEntity : BaseEntity
{
    // Pricing snapshot
    public required decimal TotalPrice { get; set; }


    // Scheduling snapshot
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }


    // Capacity snapshot
    public required int RequiredEmployees { get; set; }
    public required int RequiredBays { get; set; } = 1;


    // Hold/expiry (used while PendingPayment)
    public DateTime? ReservationExpiresAtUtc { get; set; }


    public required BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public required ServiceMode ServiceMode { get; set; }


    // CUSTOMER
    public required int CustomerId { get; set; }
    public ApplicationUserEntity Customer { get; set; } = null!;


    // SHOP LOCATION (required for knowledge about the home base of the mobile team)
    public required int ShopLocationId { get; set; }
    public LocationEntity ShopLocation { get; set; } = null!;


    // Mobile address (required when ServiceMode = Mobile)
    public int? ServiceAddressId { get; set; }
    public AddressEntity? ServiceAddress { get; set; }


    public required int ServicePackageId { get; set; }
    public ServicePackageEntity ServicePackage { get; set; } = null!;


    // REVIEW
    public ReviewEntity? Review { get; set; }



    public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

    // Employees assigned to execute this booking (N employees possible)
    public IReadOnlyCollection<BookingEmployeeAssignmentEntity> EmployeeAssignments { get; private set; } = new List<BookingEmployeeAssignmentEntity>();

    public IReadOnlyCollection<BookingItemEntity> BookingItems { get; private set; } = new List<BookingItemEntity>();

    public IReadOnlyCollection<PaymentTransactionEntity> PaymentTransactions { get; private set; } = new List<PaymentTransactionEntity>();
}
