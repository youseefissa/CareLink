namespace CareLink.Application.DTOs.Notification
{
    public class SendNotificationDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}