using CareLink.Domain.Enums;

namespace CareLink.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}