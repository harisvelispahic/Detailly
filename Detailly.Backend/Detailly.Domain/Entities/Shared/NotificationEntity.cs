using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Shared
{
    public class NotificationEntity : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;

        //public bool IsRead { get; set; } = false;

        // Foreign keys
        public int ApplicationUserId { get; set; }   // FK to ApplicationUser
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;
    }
}
