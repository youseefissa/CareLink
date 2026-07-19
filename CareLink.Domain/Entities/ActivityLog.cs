using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class ActivityLog : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public string ActivityType { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}