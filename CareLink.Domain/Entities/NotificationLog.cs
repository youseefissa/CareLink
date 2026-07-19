using CareLink.Domain.Entities.Common;
using CareLink.Domain.Enums;

namespace CareLink.Domain.Entities
{
    public class NotificationLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public DateTime? DeliveredAt { get; set; }
    }
}