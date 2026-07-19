using CareLink.Domain.Entities.Common;

namespace CareLink.Domain.Entities
{
    public class MedicationConfirmation : BaseEntity
    {
        public Guid PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; } = null!;

        public string MedicationName { get; set; } = string.Empty;
        public DateTime ScheduledTime { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public bool IsConfirmed { get; set; }
    }
}