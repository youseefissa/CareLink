using CareLink.Domain.Entities.Common;
using CareLink.Domain.Enums;

namespace CareLink.Domain.Entities
{
    public class Alert : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}