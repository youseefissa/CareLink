namespace CareLink.Application.DTOs.ActivityLog
{
    public class CreateActivityLogDto
    {
        public Guid PatientProfileId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}