namespace CareLink.Application.DTOs.ActivityLog
{
    public class ActivityLogDto
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}