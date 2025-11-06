using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Detailly.Domain.Entities.Shared;

public class NotificationEntity : BaseEntity
{
    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(2000)]
    public required string Message { get; set; }

    //public bool IsRead { get; set; } = false;


    // Foreign keys
    public required int ApplicationUserId { get; set; }   // FK to ApplicationUser
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;
}
