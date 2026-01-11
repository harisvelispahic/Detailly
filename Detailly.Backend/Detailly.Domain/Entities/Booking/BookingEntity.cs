using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Domain.Entities.Booking;

public class BookingEntity : BaseEntity
{
    public required decimal TotalPrice { get; set; }
    public required BookingStatus Status { get; set; }
    public string? Notes { get; set; }

    // PAYMENT (optional until paid)
    public PaymentTransactionEntity? PaymentTransaction { get; set; }


    // CUSTOMER
    public required int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;

    // EMPLOYEE (assigned later)
    public int? EmployeeId { get; set; }
    public ApplicationUserEntity? Employee { get; set; }

    public required int ServicePackageId { get; set; }

    public ServicePackageEntity ServicePackage { get; set; } = null!;
    
    // SCHEDULING
    public required int TimeSlotId { get; set; }
    public TimeSlotEntity TimeSlot { get; set; } = null!;

    // REVIEW
    public ReviewEntity? Review { get; set; }

    public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

}
