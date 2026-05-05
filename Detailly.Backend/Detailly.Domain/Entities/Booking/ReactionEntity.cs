using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Booking;

public class ReactionEntity : BaseEntity
{
    public int ServicePackageId { get; set; }
    public int CustomerId { get; set; }
    public ReactionType ReactionType { get; set; }

    public ServicePackageEntity ServicePackage { get; set; } = null!;
    public ApplicationUserEntity Customer { get; set; } = null!;
}
